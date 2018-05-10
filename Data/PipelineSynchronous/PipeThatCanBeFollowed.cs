using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Data.PipelineSynchronous
{
	public class PipeThatCanBeFollowed<TIn, TOut>
	{
		public delegate void Notify(PossibleResult<TOut> result);

		private readonly List<IHandleResult<TOut>> _listeners = new List<IHandleResult<TOut>>();
		protected Action<TIn> _impl;

		protected void _Err(Exception error)
		{
			ResultGenerated?.Invoke(PossibleResult<TOut>.Of(error));
		}

		protected void _Notify(TOut result)
		{
			ResultGenerated?.Invoke(PossibleResult<TOut>.Of(result));
		}

		protected void _Finish()
		{
			ResultGenerated?.Invoke(PossibleResult<TOut>.Done());
		}

		public event Notify ResultGenerated;

		protected string _Format(MethodInfo handler)
		{
			return
				$"{_FormatType(handler.ReturnType)} {_FormatType(handler.DeclaringType)}.{handler.Name}({_FormatParams(handler.GetParameters())})";
		}

		private string _FormatType(Type type)
		{
			if (type.IsGenericType)
			{
				var parameters = string.Join(", ", type.GetGenericArguments().Select(_FormatType));
				return $"{type.Name}[{parameters}]";
			}
			return type.Name;
		}

		private string _FormatParams(IEnumerable<ParameterInfo> parameters)
		{
			return string.Join(", ", parameters.Select(p => $"{_FormatType(p.ParameterType)} {p.Name}"));
		}

		protected void DescribeChildren(TextWriter output, int myLevel)
		{
			foreach (var listener in _listeners)
			{
				listener.Describe(output, myLevel + 1);
			}
		}

		public PipeThatCanBeFollowed<TOut, TNext> AndThen<TNext>(AdapterPipe<TOut, TNext> downstream)
		{
			AndThen((IHandleResult<TOut>) downstream);
			return downstream;
		}

		public void AndThen(IHandleResult<TOut> downstream)
		{
			ResultGenerated += evt => evt.Handle(downstream);
			_listeners.Add(downstream);
		}

		public PipeMiddle<TOut, TNext> AndThen<TNext>(Func<TOut, TNext> handler)
		{
			var downstream = new PipeMiddle<TOut, TNext>(handler);
			AndThen(downstream);
			return downstream;
		}

		public PipeMiddle<TOut, TNext> AndThen<TNext>(Action<TOut, Action<TNext>> handler)
		{
			var downstream = new PipeMiddle<TOut, TNext>(handler);
			AndThen(downstream);
			return downstream;
		}
	}
}
