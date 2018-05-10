using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Data.PipelineSynchronous
{
	public abstract class PipeThatCanBeFollowed<TIn, TOut>
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
				$"{handler.ReturnType.Name} {handler.DeclaringType.Name}.{handler.Name}({_FormatParams(handler.GetParameters())})";
		}

		private string _FormatParams(IEnumerable<ParameterInfo> parameters)
		{
			return string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
		}

		protected void DescribeChildren(TextWriter output, int myLevel)
		{
			foreach (var listener in _listeners)
			{
				listener.Describe(output, myLevel + 1);
			}
		}

		public void AddSegments(IHandleResult<TOut> downstream)
		{
			ResultGenerated += evt => evt.Handle(downstream);
			_listeners.Add(downstream);
		}

		public PipeMiddle<TOut, TNext> AddSegments<TNext>(Func<TOut, TNext> handler)
		{
			var downstream = new PipeMiddle<TOut, TNext>(handler);
			AddSegments(downstream);
			return downstream;
		}

		public PipeMiddle<TOut, TNext> AddSegments<TNext>(Action<TOut, Action<TNext>> handler)
		{
			var downstream = new PipeMiddle<TOut, TNext>(handler);
			AddSegments(downstream);
			return downstream;
		}
	}
}
