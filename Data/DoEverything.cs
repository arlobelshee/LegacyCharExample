using System;
using System.Collections.Generic;
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
			var orchestration = CreatePipeline(out characterTrap, out configTrap);

			orchestration.Call(fileName);

			var characterFile = characterTrap.Results[0];
			var configFile = configTrap.Results[0];

			Func<CharacterFile, List<CardData>> getTheCards = c => c.ParseCards();
			var scatter = PipelineAdapter.Scatter(getTheCards);
			var partialCardFinder = new PipeSource<CharacterFile, CardData>(scatter);
			var partialCardsTrap = new Collector<CardData>();
			partialCardFinder.AndThen(partialCardsTrap);
			partialCardFinder.Call(characterFile);
			var partialCards = partialCardsTrap.Results;

			var localCards = configFile.ParseCards();
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

		public static PipeSource<string, CharacterFile> CreatePipeline(out Collector<CharacterFile> characterTrap,
			out Collector<ConfigFile> configTrap)
		{
			var orchestration = new PipeSource<string, CharacterFile>(CharacterFile.From);
			characterTrap = new Collector<CharacterFile>();
			orchestration.AndThen(characterTrap);

			var configFileParse = orchestration.AndThen(ConfigFile.Matching);
			configTrap = new Collector<ConfigFile>();
			configFileParse.AndThen(configTrap);
			return orchestration;
		}

		private void _LocateAndTranslateFormulas(CardData card)
		{
			throw new NotImplementedException();
		}
	}
}
