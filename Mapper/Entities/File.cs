using System;
using System.IO;
using System.Xml.Serialization;
using Mapper.Utilities;

namespace Mapper.Entities
{
    
    /// <summary>
    /// Description of File.
    /// </summary>
    [XmlRoot(Namespace = "http://mapper.com/mappings")]
    public class File
	{
		[XmlAttribute]
		public string Name { get; set; }
		
		[XmlAttribute]
		public string Password { get; set; }

        [XmlIgnore]
        public string WorkingDirectory { get; private set; }
		
		public ChildItemCollection<File, Card> Cards { get; private set; }
	
		public SheetInfo InputFileInfo { get; set; }

	    public File()
	    {
	        Cards = new ChildItemCollection<File, Card>(this);
	    }

		public static File LoadXml(string filePath)
		{
            XmlValidator.ValidateMapping(filePath);
            var file = EntitySerializer.Deserialize<File>(filePath);
		    file.WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(filePath));
		    return file;
		}

        public static string ResolveRelativePath(string referencePath, string relativePath)
        {
            var uri = new Uri(Path.Combine(referencePath, relativePath));
            return Path.GetFullPath(uri.LocalPath);
        }

        public string ResolveRelativePath(string relativePath)
        {
            return ResolveRelativePath(WorkingDirectory, relativePath);
        }
    }
}
