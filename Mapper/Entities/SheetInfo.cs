using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Mapper.Utilities;

namespace Mapper.Entities
{
	/// <summary>
	/// Description of SheetInfo.
	/// </summary>
	public class SheetInfo
	{
		[XmlAttribute]
		public string Pattern { get; set; }
		
		[XmlAttribute]
		public Period Period { get; set; }
		
		[XmlAttribute]
		public string Password { get; set; }

        public string ConstructPath(string rootPath, DateTime date)
		{
			return Path.Combine(rootPath, date.ToString(Pattern));
		}

	    public Dictionary<DateTime, string> ConstructPaths(string rootPath, DateTime from, DateTime to)
	    {
	        return new DateEnumerable(Period, from, to).ToDictionary(d => d, d => ConstructPath(rootPath, d));
	    }
	}
}
