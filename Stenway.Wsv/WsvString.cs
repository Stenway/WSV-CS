namespace Stenway.Wsv
{
	public static class WsvString
	{
		public static bool isWhitespace(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			for (int i=0; i<str.Length; i++)
			{
				char c = str[i];
				if (!WsvChar.IsWhitespace(c))
				{
					return false;
				}
			}
			return true;
		}
	}
}