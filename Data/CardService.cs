using System;
using JetBrains.Annotations;

namespace Data
{
	public class CardService
	{
		[NotNull]
		public static CardService Authenticate([NotNull] string username, [NotNull] string password)
		{
			throw new NotImplementedException();
		}

		public void FetchDetailsInto(CardData card)
		{
			throw new NotImplementedException();
		}

		public void ResolveReferencesToOtherCards(CardData card)
		{
			throw new NotImplementedException();
		}
	}
}
