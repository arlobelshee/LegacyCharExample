using System;
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

		private void _AddOneTwoThree(decimal input, Action<decimal> generateOneResult)
		{
			generateOneResult(input + 1);
			generateOneResult(input + 2);
			generateOneResult(input + 3);
		}

		private static object Done()
		{
			return Evt("ResultGenerated", PossibleResult<decimal>.Done());
		}

		private static object Result(decimal value)
		{
			return Evt("ResultGenerated", PossibleResult<decimal>.Of(value));
		}

		private static object Evt(string eventName, params object[] args)
		{
			return new {EventName = eventName, Parameters = args};
		}

		[Test]
		public void ShouldAllowGeneratorFunctionsToPushSeveralArgumentsThenAutomaticallyFinishWhenTheyAreDone()
		{
			var testSubject = new PipeSegment<decimal, decimal>(_AddOneTwoThree);
			using (var monitor = testSubject.Monitor())
			{
				testSubject.Call(4);
				var expectation = new[] {Result(5), Result(6), Result(7), Done()};
				monitor.OccurredEvents.Should().BeEquivalentTo(expectation,
					options => options.WithStrictOrdering().ExcludingMissingMembers());
			}
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
				var expectation = new[] {Result(7), Done()};
				monitor.OccurredEvents.Should().BeEquivalentTo(expectation,
					options => options.WithStrictOrdering().ExcludingMissingMembers());
			}
		}
	}
}
