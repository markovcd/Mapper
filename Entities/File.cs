using System.IO;
using System.Xml.Serialization;

namespace Mapper
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
			using (var stream = new StreamReader(filePath))
			{
				var serializer = new XmlSerializer(typeof(File));
				var file = (File)serializer.Deserialize(stream);
				return file;
			}			
		}				
	}
}
