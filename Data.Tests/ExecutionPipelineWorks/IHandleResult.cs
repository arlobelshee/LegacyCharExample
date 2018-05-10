using System;

namespace Data.Tests.ExecutionPipelineWorks
{
	public interface IHandleResult<T>
	{
		void HandleData(T data);
		void HandleError(Exception error);
		void HandleDone();
	}
}
