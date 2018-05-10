using System;

namespace Data.PipelineSynchronous
{
	public interface IHandleResult<T>
	{
		void HandleData(T data);
		void HandleError(Exception error);
		void HandleDone();
	}
}
