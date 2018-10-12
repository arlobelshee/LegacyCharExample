using System;
using System.IO;

namespace Data.PipelineSynchronous
{
	public class PipeMiddle<TIn, TOut> : PipeThatCanBeFollowed<TIn, TOut>, IHandleResult<TIn>
	{
		private readonly NodeDisplay _nodeDisplay;

		public PipeMiddle(Action<TIn, Action<TOut>> handler)
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
				}
			};
			_nodeDisplay = new NodeDisplay(_Format(handler.Method));
		}

		public PipeMiddle(Func<TIn, TOut> handler)
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
			};
			_nodeDisplay = new NodeDisplay(_Format(handler.Method));
		}

		public void HandleData(TIn data)
		{
			_impl(data);
		}

		public void HandleError(Exception error)
		{
			_Err(error);
		}

		public void HandleDone()
		{
			_Finish();
		}

		public void Describe(TextWriter output, int myLevel)
		{
			_nodeDisplay.WriteTo(output, myLevel);
			DescribeChildren(output, myLevel);
		}

		public static Collector<TOut> Collect<TIn, TOut>(PipeMiddle<TIn, TOut> configPipe)
		{
			var configCollector = new Collector<TOut>();
			configPipe.AndThen(configCollector);
			return configCollector;
		}
	}
}