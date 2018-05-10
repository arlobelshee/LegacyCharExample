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
			Collector<CardData> partialCardsTrap;
			Collector<CardData> localCardsTrap;
			var orchestration = DoEverything.CreateMorePipe(out characterTrap, out configTrap, out configFileNode, out partialCardsTrap, out localCardsTrap);
			Approvals.Verify(orchestration);
		}
	}
}
