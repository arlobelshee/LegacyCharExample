using System;
using System.IO;

namespace Data.PipelineSynchronous
{
	public interface IHandleResult<T>
	{
		void HandleData(T data);
		void HandleError(Exception error);
		void HandleDone();
		void Describe(TextWriter output, int myLevel);
	}
}
