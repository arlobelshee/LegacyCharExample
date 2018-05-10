using System;
using System.IO;

namespace Data.PipelineSynchronous
{
	public abstract class AdapterPipe<TIn, TOut>: PipeThatCanBeFollowed<TIn, TOut>, IHandleResult<TIn>
	{
		public abstract void HandleData(TIn data);
		public abstract void HandleError(Exception error);
		public abstract void HandleDone();
		public abstract void Describe(TextWriter output, int myLevel);
	}
}