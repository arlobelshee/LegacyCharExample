using System;

namespace Data.Tests.ExecutionPipelineWorks
{
	public class PipeSource<TIn, TOut>
	{
		public delegate void Notify(PossibleResult<TOut> result);

		private readonly Action<TIn> _impl;
		private string _text;

		public PipeSource(Func<TIn, TOut> handler)
		{
			_impl = input =>
			{
				var result = handler(input);
				_Notify(result);
				_Finish();
			};
			_text = handler.ToString();
		}

		public PipeSource(Action<TIn, Action<TOut>> handler)
		{
			_impl = input =>
			{
				handler(input, _Notify);
				_Finish();
			};
			_text = handler.ToString();
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
		}
	}
}
