using System;
using System.Collections.Generic;

namespace Data.Tests.ExecutionPipelineWorks
{
	public class Collector<T> : PipeHead<T>, IHandleResult<T>
	{
		public List<T> Results { get; } = new List<T>();

		public void HandleData(T data)
		{
			Results.Add(data);
		}

		public void HandleError(Exception error)
		{
		}

		public void HandleDone()
		{
		}
	}
}
