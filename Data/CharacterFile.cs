using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Data
{
	public class CharacterFile
	{
		public static CharacterFile From([NotNull] string fileName)
		{
			throw new NotImplementedException();
		}

		public List<CardData> ParseCards()
		{
			return new List<CardData>();
		}

		public void ResolveFormulasToValues(CardData card, ConfigFile configFile)
		{
			throw new NotImplementedException();
		}
	}
}
