using System.Text;
namespace Stenway.Wsv
{
	public static class WsvSerializer
	{
		private static bool ContainsSpecialChar(string value)
		{
			foreach (char c in value)
			{
				if (c == '\n' || WsvChar.IsWhitespace(c) || c == '"'
						 || c == '#')
				{
					return true;
				}
			}
			return false;
		}
	
		public static void SerializeValue(StringBuilder sb, string value)
		{
			if (value==null)
			{
				sb.Append('-');
			}
			else if (value.Length == 0)
			{
				sb.Append("\"\"");
			}
			else if (value == "-")
			{
				sb.Append("\"-\"");
			}
			else if (ContainsSpecialChar(value))
			{
				sb.Append('"');
				foreach (char c in value)
				{
					if (c == '\n')
					{
						sb.Append("\"/\"");
					}
					else if(c == '"')
					{
						sb.Append("\"\"");
					}
					else
					{
						sb.Append(c);
					}
				}
				sb.Append('"');
			}
			else
			{
				sb.Append(value);
			}
		}
		
		private static void SerializeWhitespace(StringBuilder sb, string whitespace,
				bool isRequired)
		{
			if (whitespace != null && whitespace.Length > 0)
			{
				sb.Append(whitespace);
			}
			else if (isRequired)
			{
				sb.Append(" ");
			} 
		}
	
		private static void SerializeValuesWithWhitespace(StringBuilder sb,
				WsvLine line)
		{
			if (line.Values == null)
			{
				string whitespace = line.whitespaces[0];
				SerializeWhitespace(sb, whitespace, false);
				return;
			}
			
			for (int i=0; i<line.Values.Length; i++)
			{
				string whitespace = null;
				if (i < line.whitespaces.Length)
				{
					whitespace = line.whitespaces[i];
				}
				if (i == 0)
				{
					SerializeWhitespace(sb, whitespace, false);
				}
				else
				{
					SerializeWhitespace(sb, whitespace, true);
				}
	
				SerializeValue(sb, line.Values[i]);
			}
			
			if (line.whitespaces.Length >= line.Values.Length + 1)
			{
				string whitespace = line.whitespaces[line.Values.Length];
				SerializeWhitespace(sb, whitespace, false);
			}
			else if (line.comment != null && line.Values.Length > 0)
			{
				sb.Append(' ');
			}
		}
		
		private static void SerializeValuesWithoutWhitespace(StringBuilder sb, 
				WsvLine line)
		{
			if (line.Values == null)
			{
				return;
			}
			
			bool isFollowingValue = false;
			foreach (string value in line.Values)
			{
				if (isFollowingValue)
				{
					sb.Append(' ');
				}
				else
				{
					isFollowingValue = true;
				}
				SerializeValue(sb, value);
			}
	
			if (line.comment != null && line.Values.Length > 0)
			{
				sb.Append(' ');
			}
		}
		
		public static void SerializeLine(StringBuilder sb, WsvLine line)
		{
			if (line.whitespaces != null && line.whitespaces.Length > 0)
			{
				SerializeValuesWithWhitespace(sb, line);
			}
			else
			{
				SerializeValuesWithoutWhitespace(sb, line);
			}
			
			if (line.comment != null)
			{
				sb.Append('#');
				sb.Append(line.comment);
			}
		}
		
		public static string SerializeLine(WsvLine line)
		{
			StringBuilder sb = new StringBuilder();
			SerializeLine(sb, line);
			return sb.ToString();
		}
		
		public static string SerializeDocument(WsvDocument document)
		{
			StringBuilder sb = new StringBuilder();
			bool isFirstLine = true;
			foreach (WsvLine line in document.Lines)
			{
				if (!isFirstLine)
				{
					sb.Append('\n');
				}
				else
				{
					isFirstLine = false;
				}
				SerializeLine(sb, line);
			}
			return sb.ToString();
		}
		
		public static string SerializeLineNonPreserving(WsvLine line)
		{
			StringBuilder sb = new StringBuilder();
			SerializeLine(sb, line.Values);
			return sb.ToString();
		}
		
		public static string SerializeDocumentNonPreserving(WsvDocument document)
		{
			StringBuilder sb = new StringBuilder();
			bool isFirstLine = true;
			foreach (WsvLine line in document.Lines)
			{
				if (!isFirstLine)
				{
					sb.Append('\n');
				}
				else
				{
					isFirstLine = false;
				}
				SerializeLine(sb, line.Values);
			}
			return sb.ToString();
		}
		
		public static void SerializeLine(StringBuilder sb, string[] line)
		{
			bool isFirstValue = true;
			foreach (string value in line)
			{
				if (!isFirstValue)
				{
					sb.Append(' ');
				}
				else
				{
					isFirstValue = false;
				}
				SerializeValue(sb, value);
			}
		}
		
		public static string SerializeLine(params string[] line)
		{
			StringBuilder sb = new StringBuilder();
			SerializeLine(sb, line);
			return sb.ToString();
		}
		
		public static string SerializeDocument(string[][] lines)
		{
			StringBuilder sb = new StringBuilder();
			bool isFirstLine = true;
			foreach (string[] line in lines)
			{
				if (!isFirstLine)
				{
					sb.Append('\n');
				}
				else
				{
					isFirstLine = false;
				}
				SerializeLine(sb, line);
			}
			return sb.ToString();
		}
	}
}