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
			var pipeline = new PipeSource<string, CharacterFile>((f) => CharacterFile.From(f));
			var collector = new Collector<CharacterFile>();
			pipeline.AndThen(collector);
			pipeline.Call(fileName);
			var characterFile = collector.Results.First();
			var configFile = ConfigFile.Matching(characterFile);
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
