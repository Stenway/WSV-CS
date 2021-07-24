using System;
namespace Stenway.Wsv
{
	public class WsvLine {
		public string[] Values;
		
		internal string[] whitespaces;
		internal string comment;
		
		public String Comment
		{
			get
			{
				return comment;
			}
			set
			{
				ValidateComment(value);
				this.comment = value;
			}
		}
		
		
		public string[] Whitespaces
		{
			get
			{
				if (whitespaces == null)
				{
					return null;
				}
				return (String[])whitespaces.Clone();
			}
			set
			{
				ValidateWhitespaces(value);
				this.whitespaces = value;
			}
		}
				
		public WsvLine()
		{
			
		}
		
		public WsvLine(params string[] values)
		{
			Values = values;
			
			whitespaces = null;
			comment = null;
		}
	
		public WsvLine(string[] values, string[] whitespaces, string comment)
		{
			Values = values;
			
			SetWhitespaces(whitespaces);
			Comment = comment;
		}
		
		public bool HasValues()
		{
			return Values != null && Values.Length > 0;
		}
		
		public void SetValues(params string[] values)
		{
			Values = values;
		}
				
		public void SetWhitespaces(params string[] whitespaces)
		{
			Whitespaces = whitespaces;
		}
		
		public static void ValidateWhitespaces(params string[] whitespaces)
		{
			if (whitespaces != null)
			{
				foreach (string whitespace in whitespaces)
				{
					if (whitespace != null && whitespace.Length > 0 && !WsvString.isWhitespace(whitespace))
					{
						throw new ArgumentException(
								"Whitespace value contains non whitespace character or line feed");
					}
				}
			}
		}
				
		public static void ValidateComment(string comment)
		{
			if (comment != null && comment.IndexOf('\n') >= 0) {
				throw new ArgumentException(
						"Line feed in comment is not allowed");
			}
		}
		
		internal void Set(string[] values, string[] whitespaces, string comment)
		{
			Values = values;
			this.whitespaces = whitespaces;
			this.comment = comment;
		}
		
		public override string ToString()
		{
			return ToString(true);
		}
		
		public string ToString(bool preserveWhitespaceAndComment)
		{
			if (preserveWhitespaceAndComment)
			{
				return WsvSerializer.SerializeLine(this);
			}
			else
			{
				return WsvSerializer.SerializeLineNonPreserving(this);
			}
		}
		
		public static WsvLine Parse(string content)
		{
			return Parse(content, true);
		}
		
		public static WsvLine Parse(string content, bool preserveWhitespaceAndComment)
		{
			if (preserveWhitespaceAndComment)
			{
				return WsvParser.ParseLine(content);
			}
			else
			{
				return WsvParser.ParseLineNonPreserving(content);
			}
		}
		
		public static string[] ParseAsArray(string content)
		{
			return WsvParser.ParseLineAsArray(content);
		}
	}
}