using ApprovalTests;
using Data.PipelineSynchronous;
using NUnit.Framework;

namespace Data.Tests
{
	[TestFixture]
	public class DoAllTheRightThings
	{
		[Test]
		public void OrchestrationShouldBeAsExpected()
		{
			Collector<CharacterFile> characterTrap;
			Collector<ConfigFile> configTrap;
			var orchestration = DoEverything.CreatePipeline(out characterTrap, out configTrap);
			Approvals.Verify(orchestration);
		}

	}
}
