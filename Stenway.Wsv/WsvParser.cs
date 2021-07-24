using System.Collections.Generic;
namespace Stenway.Wsv
{
	class WsvParser {
		private static readonly string MULTIPLE_WSV_LINES_NOT_ALLOWED = "Multiple WSV lines not allowed";
		private static readonly string UNEXPECTED_PARSER_ERROR = "Unexpected parser error";
		
		private static WsvLine ParseLine(WsvCharIterator iterator, 
				List<string> values, List<string> whitespaces)
		{
			values.Clear();
			whitespaces.Clear();
			
			string whitespace = iterator.ReadWhitespaceOrNull();
			whitespaces.Add(whitespace);
	
			while (!iterator.IsChar('\n') && !iterator.IsEndOfText)
			{
				string value;
				if(iterator.IsChar('#'))
				{
					break;
				}
				else if(iterator.TryReadChar('"'))
				{
					value = iterator.ReadString();
				}
				else
				{
					value = iterator.ReadValue();
					if (value == "-")
					{
						value = null;
					}
				}
				values.Add(value);
	
				whitespace = iterator.ReadWhitespaceOrNull();
				if (whitespace == null)
				{
					break;
				}
				whitespaces.Add(whitespace);
			}
			
			string comment = null;
			if(iterator.TryReadChar('#'))
			{
				comment = iterator.ReadCommentText();
				if (whitespace == null)
				{
					whitespaces.Add(null);
				}
			}
			
			WsvLine newLine = new WsvLine();
			newLine.Set(values.ToArray(), whitespaces.ToArray(), comment);
			return newLine;
		}
		
		public static WsvLine ParseLine(string content)
		{
			WsvCharIterator iterator = new WsvCharIterator(content);
			List<string> values = new List<string>();
			List<string> whitespaces = new List<string>();
			
			WsvLine newLine = ParseLine(iterator, values, whitespaces);
			if (iterator.IsChar('\n'))
			{
				throw iterator.GetException(MULTIPLE_WSV_LINES_NOT_ALLOWED);
			}
			else if (!iterator.IsEndOfText)
			{
				throw iterator.GetException(UNEXPECTED_PARSER_ERROR);
			}
			
			return newLine;
		}
		
		public static WsvDocument ParseDocument(string content)
		{
			WsvDocument document = new WsvDocument();
			
			WsvCharIterator iterator = new WsvCharIterator(content);
			List<string> values = new List<string>();
			List<string> whitespaces = new List<string>();
			
			while (true)
			{
				WsvLine newLine = ParseLine(iterator, values, whitespaces);
				document.AddLine(newLine);
				
				if (iterator.IsEndOfText)
				{
					break;
				}
				else if(!iterator.TryReadChar('\n'))
				{
					throw iterator.GetException(UNEXPECTED_PARSER_ERROR);
				}
			}
			
			if (!iterator.IsEndOfText)
			{
				throw iterator.GetException(UNEXPECTED_PARSER_ERROR);
			}
	
			return document;
		}
		
		public static WsvLine ParseLineNonPreserving(string content)
		{
			string[] values = ParseLineAsArray(content);
			return new WsvLine(values);
		}
		
		public static WsvDocument ParseDocumentNonPreserving(string content)
		{
			WsvDocument document = new WsvDocument();
			
			WsvCharIterator iterator = new WsvCharIterator(content);
			List<string> values = new List<string>();
			
			while (true)
			{
				string[] lineValues = ParseLineAsArray(iterator, values);
				WsvLine newLine = new WsvLine(lineValues);
				document.AddLine(newLine);
				
				if (iterator.IsEndOfText)
				{
					break;
				}
				else if(!iterator.TryReadChar('\n'))
				{
					throw iterator.GetException(UNEXPECTED_PARSER_ERROR);
				}
			}
			
			if (!iterator.IsEndOfText)
			{
				throw iterator.GetException(UNEXPECTED_PARSER_ERROR);
			}
	
			return document;
		}
		
		public static string[][] ParseDocumentAsJaggedArray(string content)
		{
			WsvCharIterator iterator = new WsvCharIterator(content);
			List<string> values = new List<string>();
			List<string[]> lines = new List<string[]>();
			
			while (true)
			{
				string[] newLine = ParseLineAsArray(iterator, values);
				lines.Add(newLine);
				
				if (iterator.IsEndOfText)
				{
					break;
				}
				else if(!iterator.TryReadChar('\n'))
				{
					throw iterator.GetException(UNEXPECTED_PARSER_ERROR);
				}
			}
			
			if (!iterator.IsEndOfText)
			{
				throw iterator.GetException(UNEXPECTED_PARSER_ERROR);
			}
			
			return lines.ToArray();
		}
		
		public static string[] ParseLineAsArray(string content)
		{
			WsvCharIterator iterator = new WsvCharIterator(content);
			List<string> values = new List<string>();
			string[] result = ParseLineAsArray(iterator, values);
			if (iterator.IsChar('\n'))
			{
				throw iterator.GetException(MULTIPLE_WSV_LINES_NOT_ALLOWED);
			}
			else if (!iterator.IsEndOfText)
			{
				throw iterator.GetException(UNEXPECTED_PARSER_ERROR);
			}
			return result;
		}
		
		private static string[] ParseLineAsArray(WsvCharIterator iterator, List<string> values)
		{
			values.Clear();
			iterator.SkipWhitespace();
	
			while (!iterator.IsChar('\n') && !iterator.IsEndOfText)
			{
				string value;
				if(iterator.IsChar('#'))
				{
					break;
				}
				else if(iterator.TryReadChar('"'))
				{
					value = iterator.ReadString();
				}
				else
				{
					value = iterator.ReadValue();
					if (value == "-")
					{
						value = null;
					}
				}
				values.Add(value);
	
				if (!iterator.SkipWhitespace())
				{
					break;
				}
			}
			
			if (iterator.TryReadChar('#'))
			{
				iterator.SkipCommentText();
			}
			
			return values.ToArray();
		}
	}
}