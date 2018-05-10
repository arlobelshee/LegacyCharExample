using JetBrains.Annotations;

namespace Data
{
	public class DoEverything
	{
		public CharacterData MakeAllTheViewModels([NotNull] string fileName)
		{
			return new CharacterData();
		}
	}
}
