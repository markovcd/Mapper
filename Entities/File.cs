using System.IO;
using System.Collections.Generic;
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
		
		public List<Card> Cards { get; set; }
	
		public SheetInfo InputFileInfo { get; set; }
		
		static public File LoadXml(string filePath)
		{
			var stream = new StreamReader(filePath);
			var serializer = new XmlSerializer(typeof(File));
			var file = (File)serializer.Deserialize(stream);
			stream.Close();
			
			file.SetParents();
			
			return file;
		}
		
		private void SetParents()
		{
			foreach (var card in Cards)
			{
				card.File = this;
				
				foreach (var sample in card.Samples)
				{
					sample.Card = card;
					
					foreach (var mapping in sample.Mappings)
						mapping.Sample = sample;
				}
			}
			
		}
	
		public void SaveXml(string filePath)
		{
			var serializer = new XmlSerializer(typeof(File));
			var stream = new StreamWriter(filePath);
			serializer.Serialize(stream, this);
		}
	}
}
