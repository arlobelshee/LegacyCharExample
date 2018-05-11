using System.IO;

namespace Data.PipelineSynchronous
{
	public class NodeDisplay
	{
		private readonly string _text;

		public NodeDisplay(string text)
		{
			_text = text;
		}

		public void WriteTo(TextWriter output, int myLevel)
		{
			for (var i = 0; i < myLevel; i++)
			{
				output.Write(" |");
			}
			output.Write("--> ");
			output.WriteLine(_text);
		}
	}
}
