using System;
using Stenway.ReliableTxt;

namespace Stenway.Wsv
{
	class WsvCharIterator : ReliableTxtCharIterator
	{
		public WsvCharIterator(string text) : base(text)
		{
			
		}
		
		public bool IsWhitespace()
		{
			if (IsEndOfText) return false;
			return WsvChar.IsWhitespace(chars[index]);
		}
		
		public string ReadCommentText()
		{
			int startIndex = index;
			while (true)
			{
				if (IsEndOfText) break;
				if (chars[index] == '\n') break;
				index++;
			}
			return ReliableTxtUtils.GetString(chars,startIndex,index-startIndex);
		}
		
		public void SkipCommentText()
		{
			while (true)
			{
				if (IsEndOfText) break;
				if (chars[index] == '\n') break;
				index++;
			}
		}
	
		public string ReadWhitespaceOrNull()
		{
			int startIndex = index;
			while (true)
			{
				if (IsEndOfText) break;
				int c = chars[index];
				if (c == '\n') break;
				if (!WsvChar.IsWhitespace(c)) break;
				index++;
			}
			if (index == startIndex) return null;
			return ReliableTxtUtils.GetString(chars,startIndex,index-startIndex);
		}
		
		public bool SkipWhitespace()
		{
			int startIndex = index;
			while (true)
			{
				if (IsEndOfText) break;
				int c = chars[index];
				if (c == '\n') break;
				if (!WsvChar.IsWhitespace(c)) break;
				index++;
			}
			return index > startIndex;
		}
	
		public string ReadString()
		{
			sb.Clear();
			while (true)
			{
				if (IsEndOfText || IsChar('\n'))
				{
					throw GetException("String not closed");
				}
				int c = chars[index];
				if (c == '"') {
					index++;
					if (TryReadChar('"'))
					{
						sb.Append('"');
					}
					else if(TryReadChar('/'))
					{
						if (!TryReadChar('"'))
						{
							throw GetException("Invalid string line break");
						}
						sb.Append('\n');
					}
					else if (IsWhitespace() || IsChar('\n') || IsChar('#') || IsEndOfText )
					{
						break;
					}
					else
					{
						throw GetException("Invalid character after string");
					}
				}
				else
				{
					sb.Append(Char.ConvertFromUtf32(c));
					index++;
				}
			}
			return sb.ToString();
		}
	
		public string ReadValue()
		{
			int startIndex = index;
			while (true)
			{
				if (IsEndOfText)
				{
					break;
				}
				int c = chars[index];
				if (WsvChar.IsWhitespace(c) || c == '\n' || c == '#')
				{
					break;
				}
				if (c == '\"')
				{
					throw GetException("Invalid double quote in value");
				}
				index++;
			}
			if (index == startIndex)
			{
				throw GetException("Invalid value");
			}
			return ReliableTxtUtils.GetString(chars,startIndex,index-startIndex);
		}
		
		public WsvParserException GetException(string message)
		{
			int[] lineInfo = GetLineInfo();
			return new WsvParserException(index, lineInfo[0], lineInfo[1], message);
		}
	}
}