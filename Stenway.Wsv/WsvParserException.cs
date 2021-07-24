using System;

namespace Stenway.Wsv
{
	public class WsvParserException : Exception
	{
		public readonly int Index;
		public readonly int LineIndex;
		public readonly int LinePosition;
		
		public WsvParserException(int index, int lineIndex, int linePosition, string message) : 
			base(string.Format("{0} ({1}, {2})", message, lineIndex + 1, linePosition + 1))
		{
			Index = index;
			LineIndex = lineIndex;
			LinePosition = linePosition;
		}
	}
}