using System;

namespace Data.Tests.ExecutionPipelineWorks
{
	public class PossibleResult<T>
	{
		public T Data;
		public Exception Error;
		public EventType Type;

		public static PossibleResult<T> Of(T result)
		{
			return new PossibleResult<T> {Type = EventType.Data, Data = result};
		}

		public static PossibleResult<T> Done()
		{
			return new PossibleResult<T> {Type = EventType.Done};
		}
	}
}
