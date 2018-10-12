using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Data.PipelineSynchronous
{
	public class PipeSource<TIn, TOut> : PipeThatCanBeFollowed<TIn, TOut>
	{
		private readonly NodeDisplay _nodeDisplay;

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
			_nodeDisplay = new NodeDisplay(_Format(handler.Method));
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
			_nodeDisplay = new NodeDisplay(_Format(handler.Method));
		}

		public override string ToString()
		{
			var output = new StringWriter();
			_Describe(output, 0);
			return output.ToString();
		}

		private void _Describe(TextWriter output, int myLevel)
		{
			_nodeDisplay.WriteTo(output, myLevel);
			DescribeChildren(output, myLevel);
		}

		public void Call(TIn input)
		{
			_impl(input);
		}

		public IApprovalWriter ToNewString()
		{
			throw new NotImplementedException();
		}
	}
}
