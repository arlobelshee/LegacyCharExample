using System;

namespace Data.Tests.ExecutionPipelineWorks
{
	public class PipeSegment<TIn, TOut>
	{
		public delegate void Notify(TOut result);

		private Action<TIn> _impl;
		private string _text;

		public PipeSegment(Func<TIn, TOut> handler)
		{
			_impl = input =>
			{
				var result = handler(input);
				_Notify(result);
			};
			_text = handler.ToString();
		}

		private void _Notify(TOut result)
		{
			ResultGenerated?.Invoke(result);
		}

		public event Notify ResultGenerated;

		public void Call(TIn input)
		{
			_impl(input);
		}
	}
}