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
			var orchestration = new PipeSource<string, CharacterFile>(CharacterFile.From);
			var trap = new Collector<CharacterFile>();
			orchestration.AndThen(trap);

			var configFileParse = orchestration.AndThen(ConfigFile.Matching);
			var configTrap = new Collector<ConfigFile>();
			configFileParse.AndThen(configTrap);

			orchestration.Call(fileName);

			var characterFile = trap.Results[0];
			var configFile = configTrap.Results[0];

			var partialCards = characterFile.ParseCards();
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
			return new CharacterData(localCards.Concat(partialCards).Select(CardViewModel.From));
		}

		private void _LocateAndTranslateFormulas(CardData card)
		{
			throw new System.NotImplementedException();
		}
	}
}
