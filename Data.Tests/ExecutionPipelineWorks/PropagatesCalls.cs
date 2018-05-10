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
				var expectation = new object[] {new {EventName = "ResultGenerated", Parameters = new object[] {7M}}};
				monitor.OccurredEvents.Should().BeEquivalentTo(expectation, options => options.ExcludingMissingMembers());
			}
		}
	}
}
