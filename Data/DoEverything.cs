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
			var characterPipe =
				MakePipe(out var characterCollector, out var configCollector, out var partialCardsCollector, out var configCardsCollector);
			characterPipe.Call(fileName);
			var characterFile = characterCollector.Results.First();
			var configFile = configCollector.Results.First();
			List<CardData> partialCards = partialCardsCollector.Results.First();
			var localCards = configCardsCollector.Results.First();
			var compendiumServicePipe = new PipeSource<Tuple<string, string>, CompendiumService>(t =>CompendiumService.Authenticate(t.Item1,t.Item2));
			compendiumServicePipe.AndThen(new Collector<CompendiumService>());
			CompendiumService compendiumService = CompendiumService.Authenticate(username, password);

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

		public static PipeSource<string, CharacterFile> MakePipe(out Collector<CharacterFile> characterCollector,
			out Collector<ConfigFile> configCollector, out Collector<List<CardData>> partialCardsCollector,
			out Collector<List<CardData>> configCardsCollector)
		{
			var characterPipe = new PipeSource<string, CharacterFile>(CharacterFile.From);
			characterCollector = new Collector<CharacterFile>();
			var configPipe = characterPipe.AndThen(ConfigFile.Matching);
			partialCardsCollector = new Collector<List<CardData>>();
			configCardsCollector = new Collector<List<CardData>>();
			characterPipe.AndThen(cf => cf.ParseCards())
				.AndThen(partialCardsCollector);
			configPipe.AndThen(cf => cf.ParseCards())
				.AndThen(configCardsCollector);

			configCollector = PipeMiddle<CharacterFile,ConfigFile>.Collect(configPipe);
			characterPipe.AndThen(characterCollector);

			return characterPipe;
		}

		private void _LocateAndTranslateFormulas(CardData card)
		{
			throw new NotImplementedException();
		}
	}
}
