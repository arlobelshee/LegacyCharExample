using ApprovalTests;
using ApprovalTests.Reporters;
using Data.PipelineSynchronous;
using NUnit.Framework;

namespace Data.Tests
{
	[TestFixture]
	[UseReporter(typeof(VisualStudioReporter))]

	public class CheckEverything
	{
		[Test]
		public void TestPipeline()
		{
			var pipeSource = DoEverything.MakePipe(out var characterCollector, out var configCollector);
			Approvals.Verify( pipeSource.ToNewString());
		}

	}
}
