using System;
using System.Collections.Generic;

namespace Data.PipelineSynchronous
{
	internal static class PipelineAdapter
	{
		public static Action<TIn, Action<TOut>> Scatter<TIn, TOut>(Func<TIn, List<TOut>> getAllItems)
		{
			return (source, handleData) =>
			{
				getAllItems(source)
					.ForEach(handleData);
			};
		}

		public static PipeSource<TIn, TOut> ScatterAsPipeSource<TIn, TOut>(Func<TIn, List<TOut>> getTheCards)
		{
			return new PipeSource<TIn, TOut>(Scatter(getTheCards));
		}
	}
}
