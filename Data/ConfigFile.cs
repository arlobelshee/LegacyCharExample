using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Data
{
	public class ConfigFile
	{
		public List<CardData> ParseCards()
		{
			return new List<CardData>();
		}

		public static ConfigFile Matching([NotNull] CharacterFile characterFile)
		{
			throw new NotImplementedException();
		}
	}
}
