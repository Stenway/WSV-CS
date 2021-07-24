using System;
using System.Collections.Generic;
using Stenway.ReliableTxt;

namespace Stenway.Wsv
{
	public class WsvDocument {
		public readonly List<WsvLine> Lines = new List<WsvLine>();
		
		private ReliableTxtEncoding encoding;
		
		public ReliableTxtEncoding Encoding 
		{
			get { return encoding; } 
			set
			{
				this.encoding = value;
			}
		}
		
		public WsvDocument() : this(ReliableTxtEncoding.Utf8)
		{
			
		}
		
		public WsvDocument(ReliableTxtEncoding encoding)
		{
			Encoding = encoding;
		}
				
		public void AddLine(params string[] values)
		{
			AddLine(new WsvLine(values));
		}
		
		public void AddLine(string[] values, string[] whitespaces, string comment)
		{
			AddLine(new WsvLine(values, whitespaces, comment));
		}
	
		public void AddLine(WsvLine line)
		{
			Lines.Add(line);
		}
		
		public WsvLine this[int index]
	    {
	        get
	        {
	        	return Lines[index];
	        }
	    }
		
		public string[][] ToArray()
		{
			string[][] array = new string[Lines.Count][];
			for (int i=0; i<Lines.Count; i++)
			{
				array[i] = Lines[i].Values;
			}
			return array;
		}
	
		public override string ToString()
		{
			return ToString(true);
		}
		
		public string ToString(bool preserveWhitespaceAndComments)
		{
			if (preserveWhitespaceAndComments)
			{
				return WsvSerializer.SerializeDocument(this);
			}
			else
			{
				return WsvSerializer.SerializeDocumentNonPreserving(this);
			}
		}
	
		public void Save(string filePath)
		{
			string content = ToString();
			ReliableTxtDocument.Save(content, encoding, filePath);
		}
	
		public static WsvDocument Load(string filePath)
		{
			return Load(filePath, true);
		}
		
		public static WsvDocument Load(string filePath, bool preserveWhitespaceAndComments)
		{
			ReliableTxtDocument txt = ReliableTxtDocument.Load(filePath);
			WsvDocument document = Parse(txt.Text, preserveWhitespaceAndComments);
			document.encoding = txt.Encoding;
			return document;
		}
	
		public static WsvDocument Parse(string content)
		{
			return Parse(content, true);
		}
		
		public static WsvDocument Parse(string content, bool preserveWhitespaceAndComments)
		{
			if (preserveWhitespaceAndComments)
			{
				return WsvParser.ParseDocument(content);
			}
			else
			{
				return WsvParser.ParseDocumentNonPreserving(content);
			}
		}
		
		public static string[][] ParseAsJaggedArray(string content)
		{
			return WsvParser.ParseDocumentAsJaggedArray(content);
		}
	}
}