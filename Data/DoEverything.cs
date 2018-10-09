﻿using System.Linq;
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
			var characterPipe = new PipeSource<string, CharacterFile>(CharacterFile.From);
			var configCollector = new Collector<ConfigFile>();
			var characterCollector = new Collector<CharacterFile>();
			var configPipe = characterPipe.AndThen(ConfigFile.Matching);
			characterPipe.AndThen(characterCollector);
			configPipe.AndThen(configCollector);
			characterPipe.Call(fileName);
			var characterFile = characterCollector.Results.First();
			var configFile = configCollector.Results.First();
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
