using System.Xml.Serialization;
using Mapper.Utilities;

namespace Mapper.Entities
{
	/// <summary>
	/// Description of File.
	/// </summary>
	public class File
	{
		[XmlAttribute]
		public string Name { get; set; }
		
		[XmlAttribute]
		public string Password { get; set; }
		
		public ChildItemCollection<File, Card> Cards { get; private set; }
	
		public SheetInfo InputFileInfo { get; set; }

	    public File()
	    {
	        Cards = new ChildItemCollection<File, Card>(this);
	    }

		static public File LoadXml(string filePath)
		{
            XmlValidator.ValidateMapping(filePath);
            return XmlDeserializer.LoadXml<File>(filePath);
        }				
	}
}
