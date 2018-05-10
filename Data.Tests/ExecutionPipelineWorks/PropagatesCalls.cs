using FluentAssertions;
using NUnit.Framework;

namespace Data.Tests.ExecutionPipelineWorks
{
	[TestFixture]
	public class PropagatesCalls
	{
		private decimal _lastInputSeen;

		private decimal _AddThree(decimal input)
		{
			_lastInputSeen = input;
			return input + 3;
		}

		[Test]
		public void ShouldExecuteTheCodeForThisNode()
		{
			var testSubject = new PipeSegment<decimal, decimal>(_AddThree);
			testSubject.Call(4);
			_lastInputSeen.Should().Be(4);
		}

		[Test]
		public void ShouldSendTheResultToAnyListeners()
		{
			var testSubject = new PipeSegment<decimal, decimal>(_AddThree);
			using (var monitor = testSubject.Monitor())
			{
				testSubject.Call(4);
				var expectation = new[] {Evt("ResultGenerated", 7M), Evt("DoneForThisPass")};
				monitor.OccurredEvents.Should().BeEquivalentTo(expectation, options => options.WithStrictOrdering().ExcludingMissingMembers());
			}
		}

		private static object Evt(string eventName, params object[] args)
		{
			return new {EventName = eventName, Parameters = args};
		}
	}
}
