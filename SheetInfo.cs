using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Mapper
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
	
		public string ConstructPath(string rootPath)
		{
			return PathNormalizer.Normalize(rootPath + Path.DirectorySeparatorChar + PathNormalizer.CleanQuotes(Pattern));
		}
		
		public string ConstructPath(string rootPath, DateTime date)
		{
			return PathNormalizer.Normalize(rootPath + Path.DirectorySeparatorChar + date.ToString(Pattern));
		}

	    public Dictionary<DateTime, string> ConstructPaths(string rootPath, DateTime from, DateTime to)
	    {
            var dates = new DateEnumerable(Period, from, to);
	        return dates.ToDictionary(d => d, d => ConstructPath(rootPath, d));
	    }
	}
}
