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
			testSubject.AddSegments(new Collector<int>());
			Approvals.Verify(testSubject);
		}

		private int _One(decimal arg)
		{
			throw new System.NotImplementedException();
		}
	}
}
