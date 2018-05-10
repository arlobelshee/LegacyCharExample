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
			PipeMiddle<CharacterFile, ConfigFile> configFileNode;
			var orchestration = DoEverything.CreatePipeline(out characterTrap, out configTrap, out configFileNode);
			Approvals.Verify(orchestration);
		}
	}
}
