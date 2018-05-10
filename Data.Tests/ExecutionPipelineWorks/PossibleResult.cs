using System;

namespace Data.Tests.ExecutionPipelineWorks
{
	public class PossibleResult<T>
	{
		public T Data;
		public Exception Error;
		public EventType Type;

		public void Handle(IHandleResult<T> handler)
		{
			switch (Type)
			{
				case EventType.Data:
					handler.HandleData(Data);
					break;
				case EventType.Done:
					handler.HandleDone();
					break;
				case EventType.ErrorAndDone:
					handler.HandleError(Error);
					handler.HandleDone();
					break;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public static PossibleResult<T> Of(T result)
		{
			return new PossibleResult<T> {Type = EventType.Data, Data = result};
		}

		public static PossibleResult<T> Done()
		{
			return new PossibleResult<T> {Type = EventType.Done};
		}

		public static PossibleResult<T> Of(Exception error)
		{
			return new PossibleResult<T> {Type = EventType.ErrorAndDone, Error = error};
		}
	}
}
