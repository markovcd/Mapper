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

	    private static string NormalizePath(string path)
	    {
	        return Path.GetFullPath(new Uri(path).LocalPath)
	            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
	    }

	    public string ConstructPath(string rootPath, DateTime date)
		{
			return NormalizePath(rootPath + Path.DirectorySeparatorChar + date.ToString(Pattern));
		}

	    public Dictionary<DateTime, string> ConstructPaths(string rootPath, DateTime from, DateTime to)
	    {
            var dates = new DateEnumerable(Period, from, to);
	        return dates.ToDictionary(d => d, d => ConstructPath(rootPath, d));
	    }
	}
}
