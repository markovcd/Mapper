using System;
using System.Collections.Generic;
using System.Xml.Serialization;

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

            var constructor = new FileConstructor(SourcePath, TargetPath, file, Append);
		    constructor.FileAdding += (s, e) => Console.Write(e.FilePath);
		    constructor.FileAdded += (s, e) => Console.WriteLine('.');
            constructor.AddFiles(From, To);
			constructor.Dispose();
		}
	}
	
	public class ConfigList
	{
		public List<Config> Configs;
		
		public void Add(Config config)
		{
			if (Configs == null) Configs = new List<Config>();
			Configs.Add(config);		
		}
		
		public void Execute()
		{
			foreach (var config in Configs) 
				config.Execute();
		}
	}
}
