using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Data.PipelineSynchronous
{
	public class PipeSource<TIn, TOut>
	{
		public delegate void Notify(PossibleResult<TOut> result);

		private readonly Action<TIn> _impl;
		private readonly string _text;
		private readonly List<IHandleResult<TOut>> _listeners = new List<IHandleResult<TOut>>();

		public PipeSource(Func<TIn, TOut> handler)
		{
			_impl = input =>
			{
				TOut result;
				try
				{
					result = handler(input);
				}
				catch (Exception err)
				{
					_Err(err);
					return;
				}
				_Notify(result);
				_Finish();
			};
			_text = _Format(handler.Method);
		}

		public PipeSource(Action<TIn, Action<TOut>> handler)
		{
			_impl = input =>
			{
				try
				{
					handler(input, _Notify);
				}
				catch (Exception err)
				{
					_Err(err);
					return;
				}
				_Finish();
			};
			_text = _Format(handler.Method);
		}

		private string _Format(MethodInfo handler)
		{
			return
				$"{handler.ReturnType.Name} {handler.DeclaringType.Name}.{handler.Name}({_FormatParams(handler.GetParameters())})";
		}

		private string _FormatParams(IEnumerable<ParameterInfo> parameters)
		{
			return string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
		}

		public override string ToString()
		{
			var output = new StringWriter();
			_Describe(output, 0);
			return output.ToString();
		}

		private void _Describe(TextWriter output, int myLevel)
		{
			for (var i = 0; i < myLevel; i++)
			{
				output.Write(" |");
			}
			output.Write("--> ");
			output.WriteLine(_text);
			foreach (var listener in _listeners)
			{
				listener.Describe(output, myLevel + 1);
			}
		}

		private void _Err(Exception error)
		{
			ResultGenerated?.Invoke(PossibleResult<TOut>.Of(error));
		}

		private void _Notify(TOut result)
		{
			ResultGenerated?.Invoke(PossibleResult<TOut>.Of(result));
		}

		private void _Finish()
		{
			ResultGenerated?.Invoke(PossibleResult<TOut>.Done());
		}

		public event Notify ResultGenerated;

		public void Call(TIn input)
		{
			_impl(input);
		}

		public void AddSegments(IHandleResult<TOut> downstream)
		{
			ResultGenerated += evt => evt.Handle(downstream);
			_listeners.Add(downstream);
		}
	}
}
