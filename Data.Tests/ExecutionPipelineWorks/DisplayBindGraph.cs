using System;
using ApprovalTests;
using Data.PipelineSynchronous;
using NUnit.Framework;

namespace Data.Tests.ExecutionPipelineWorks
{
	[TestFixture]
	public class DisplayBindGraph
	{
		[Test]
		public void ShouldDisplayBindGraph()
		{
			var testSubject = new PipeSource<decimal, int>(_One);
			testSubject.AddSegments(_Two).AddSegments(new Collector<float>());
			testSubject.AddSegments<string>(_Three);
			Approvals.Verify(testSubject);
		}

		private int _One(decimal arg)
		{
			throw new System.NotImplementedException();
		}

		private float _Two(int arg)
		{
			throw new System.NotImplementedException();
		}

		private void _Three(int arg, Action<string> generateOne)
		{
			throw new System.NotImplementedException();
		}
	}
}
