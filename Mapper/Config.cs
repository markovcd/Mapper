using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mapper.Entities;
using Mapper.Utilities;

namespace Mapper
{
    /// <summary>
    /// Description of Config.
    /// </summary>  
    public class Config
	{
		[XmlAttribute]
		public DateTime From { get; set; }
		
		[XmlAttribute]
		public DateTime To { get; set; }
		
		[XmlAttribute]
		public string ConfigPath { get; set; }
		
		[XmlAttribute]
		public string SourcePath { get; set; }

        [XmlAttribute]
        public string TemplatePath { get; set; }
		
		[XmlAttribute]
		public string TargetPath { get; set; }
		
		[XmlAttribute]
		public bool Append { get; set; }
		
		public void Execute()
		{
			var file = File.LoadXml(ConfigPath);
            if (!string.IsNullOrEmpty(TemplatePath)) file.Name = TemplatePath;

            var mapper = new ExcelMapper(SourcePath, TargetPath, file, Append);
		    mapper.FileAdding += (s, e) => Console.Write(e.FilePath);
		    mapper.FileAdded += (s, e) => Console.WriteLine('.');
            mapper.AddFiles(From, To);
			mapper.Dispose();
		}
	}

    [XmlRoot(Namespace = "http://mapper.com/configs")]
    public class ConfigList
	{
		public List<Config> Configs;
		
		public void Execute()
		{
			foreach (var config in Configs) 
				config.Execute();
		}
		
		static public ConfigList LoadXml(string filePath)
		{
            XmlValidator.ValidateConfig(filePath);
            return XmlDeserializer.LoadXml<ConfigList>(filePath);
		}
	}
}
