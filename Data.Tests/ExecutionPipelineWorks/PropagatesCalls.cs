using System;
using System.Collections.Generic;
using System.IO;
using Data.PipelineSynchronous;
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

		private decimal _ThowException(decimal arg)
		{
			throw new ArgumentException("The right one.");
		}

		private void _ThowExceptionOnSecondCall(decimal input, Action<decimal> generateOneResult)
		{
			generateOneResult(input + 1);
			throw new ArgumentException("The right one.");
		}

		private class Monitor : IHandleResult<decimal>
		{
			public readonly List<object> OccurredEvents = new List<object>();

			public Monitor(PipeThatCanBeFollowed<decimal, decimal> testSubject)
			{
				testSubject.AndThen(this);
			}

			public void HandleData(decimal data)
			{
				OccurredEvents.Add(Result(data));
			}

			public void HandleError(Exception error)
			{
				throw new NotImplementedException();
			}

			public void HandleDone()
			{
				OccurredEvents.Add(Done());
			}

			public void Describe(TextWriter output, int myLevel)
			{
				throw new NotImplementedException();
			}

			internal class OccurredEvent
			{
			}
		}

		[Test]
		public void CanGatherResultsEasily()
		{
			var testSubject = new PipeSource<decimal, decimal>(_AddThree);
			var results = new Collector<decimal>();
			testSubject.AndThen(results);
			testSubject.Call(4);
			results.Results.Should()
				.BeEquivalentTo(7M);
		}

		[Test]
		public void GatheringMulticastResultsRethrowsExceptions()
		{
			var testSubject = new PipeSource<decimal, decimal>(_ThowExceptionOnSecondCall);
			var results = new Collector<decimal>();
			testSubject.AndThen(results);
			testSubject.Call(4);
			results.Invoking(all =>
				{
					var r = all.Results;
				})
				.Should()
				.Throw<AggregateException>()
				.WithInnerException<ArgumentException>()
				.WithMessage("The right one.");
		}

		[Test]
		public void GatheringResultsRethrowsExceptions()
		{
			var testSubject = new PipeSource<decimal, decimal>(_ThowException);
			var results = new Collector<decimal>();
			testSubject.AndThen(results);
			testSubject.Call(4);
			results.Invoking(all =>
				{
					var r = all.Results;
				})
				.Should()
				.Throw<AggregateException>()
				.WithInnerException<ArgumentException>()
				.WithMessage("The right one.");
		}

		[Test]
		public void ShouldAllowGeneratorFunctionsToPushSeveralArgumentsThenAutomaticallyFinishWhenTheyAreDone()
		{
			var testSubject = new PipeSource<decimal, decimal>(_AddOneTwoThree);
			var monitor = new Monitor(testSubject);
			{
				testSubject.Call(4);
				var expectation = new[] {Result(5), Result(6), Result(7), Done()};
				monitor.OccurredEvents.Should()
					.BeEquivalentTo(expectation, options => options.WithStrictOrdering());
			}
		}

		[Test]
		public void ShouldExecuteTheCodeForThisNode()
		{
			var testSubject = new PipeSource<decimal, decimal>(_AddThree);
			testSubject.Call(4);
			_lastInputSeen.Should()
				.Be(4);
		}

		[Test]
		public void ShouldSendTheResultToAnyListeners()
		{
			var testSubject = new PipeSource<decimal, decimal>(_AddThree);
			var monitor = new Monitor(testSubject);
			{
				testSubject.Call(4);
				var expectation = new[] {Result(7), Done()};
				monitor.OccurredEvents.Should()
					.BeEquivalentTo(expectation, options => options.WithStrictOrdering());
			}
		}

		[Test]
		public void ShouldConnectThroughMultipleLayers()
		{
			var head = new PipeSource<decimal, decimal>(_AddThree);
			var middle = head.AndThen(_AddThree);
			var monitor = new Monitor(middle);
			{
				head.Call(4);
				var expectation = new[] {Result(10), Done()};
				monitor.OccurredEvents.Should()
					.BeEquivalentTo(expectation, options => options.WithStrictOrdering());
			}
		}
	}
}
