using System;
using System.Linq;
using Data.PipelineSynchronous;
using JetBrains.Annotations;

namespace Data
{
	public class DoEverything
	{
		[NotNull]
		public CharacterData MakeAllTheViewModels([NotNull] string fileName, [NotNull] string username,
			[NotNull] string password)
		{
			Collector<CharacterFile> characterTrap;
			Collector<ConfigFile> configTrap;
			PipeMiddle<CharacterFile, ConfigFile> configFileNode;
			Collector<CardData> partialCardsTrap;
			Collector<CardData> localCardsTrap;
			var orchestration = CreateMorePipe(out characterTrap, out configTrap, out configFileNode, out partialCardsTrap, out localCardsTrap);

			orchestration.Call(fileName);

			var characterFile = characterTrap.Results[0];
			var configFile = configTrap.Results[0];
			var partialCards = partialCardsTrap.Results;
			var localCards = localCardsTrap.Results;

			var compendiumService = CompendiumService.Authenticate(username, password);
			var cardService = CardService.Authenticate(username, password);
			foreach (var card in partialCards)
			{
				cardService.FetchDetailsInto(card);
				compendiumService.FillOutFlavorText(card);
				_LocateAndTranslateFormulas(card);
				cardService.ResolveReferencesToOtherCards(card);
			}
			foreach (var card in localCards.Concat(partialCards))
			{
				characterFile.ResolveFormulasToValues(card, configFile);
			}
			return new CharacterData(localCards.Concat(partialCards)
				.Select(CardViewModel.From));
		}

		public static PipeSource<string, CharacterFile> CreateMorePipe(out Collector<CharacterFile> characterTrap, out Collector<ConfigFile> configTrap,
			out PipeMiddle<CharacterFile, ConfigFile> configFileNode, out Collector<CardData> partialCardsTrap, out Collector<CardData> localCardsTrap)
		{
			var orchestration = new PipeSource<string, CharacterFile>(CharacterFile.From);
			characterTrap = new Collector<CharacterFile>();
			orchestration.AndThen(characterTrap);

			configFileNode = orchestration.AndThen(ConfigFile.Matching);
			configTrap = new Collector<ConfigFile>();
			configFileNode.AndThen(configTrap);

			partialCardsTrap = new Collector<CardData>();
			var partialCardFinder =
				orchestration.AndThen(PipelineAdapter.Scatter<CharacterFile, CardData>(CharacterFile.GetTheCards));
			partialCardFinder.AndThen(partialCardsTrap);

			localCardsTrap = new Collector<CardData>();
			var localCardFinder = configFileNode.AndThen(PipelineAdapter.Scatter<ConfigFile, CardData>(ConfigFile.GetTheCards));
			localCardFinder.AndThen(localCardsTrap);
			return orchestration;
		}

		private void _LocateAndTranslateFormulas(CardData card)
		{
			throw new NotImplementedException();
		}
	}
}
