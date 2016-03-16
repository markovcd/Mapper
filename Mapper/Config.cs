using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Mapper.Utilities;
using File = Mapper.Entities.File;

namespace Mapper
{
    public enum Operation
	{
		None, AppendLastDay, AppendLastMonth, AppendLastWeekend
	}
	
	/// <summary>
    /// Description of Config.
    /// </summary>  
    public class Config : ICloneable
	{
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute(DataType = "date")]
        public DateTime From { get; set; }

        [XmlAttribute(DataType = "date")]
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
		
		public void Execute(string xmlPath = null)
		{
		    xmlPath = xmlPath ?? "";

            Assert();

            var file = File.LoadXml(File.ResolveRelativePath(xmlPath, ConfigPath));

            if (!string.IsNullOrEmpty(TemplatePath))
                file.Name = File.ResolveRelativePath(xmlPath, TemplatePath);

            var mapper = new ExcelMapper(
                File.ResolveRelativePath(xmlPath, SourcePath), 
                File.ResolveRelativePath(xmlPath, TargetPath), 
                file, 
                Append);

		    mapper.FileAdding += (s, e) => Console.Write(e.FilePath);
		    mapper.FileAdded += (s, e) => Console.WriteLine('.');
            mapper.AddFiles(From, To);
			mapper.Dispose();
		}

        [DebuggerNonUserCode]
        private void Assert()
        {
            if (string.IsNullOrEmpty(ConfigPath)) throw new ArgumentNullException("ConfigPath");
            if (string.IsNullOrEmpty(SourcePath)) throw new ArgumentNullException("SourcePath");
            if (string.IsNullOrEmpty(TargetPath)) throw new ArgumentNullException("TargetPath");
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Config Clone()
        {
            return (Config)MemberwiseClone();
        }
	}

    [XmlRoot(Namespace = "http://mapper.com/configs", ElementName = "Configs")]
    public class Configs : ICloneable
    {

        [XmlAttribute]
		public Operation Operation { get; set; }

        [XmlAttribute]
        public string SourcePath { get; set; }

        [XmlAttribute(DataType = "date")]
		public DateTime From { get; set; }
		
		[XmlAttribute(DataType = "date")]
		public DateTime To { get; set; }
		
        [XmlElement("")]
        public List<Config> List { get; set; }

        [XmlIgnore]
        public string WorkingDirectory { get; private set; }

        public void Execute()
		{
        	if (!From.Equals(DateTime.MinValue)) SetFrom(From);
        	if (!To.Equals(DateTime.MinValue)) SetTo(To);
            if (!string.IsNullOrEmpty(SourcePath)) SetSourcePath(SourcePath);
        	
        	if (Operation == Operation.AppendLastDay) LastDay();
			if (Operation == Operation.AppendLastMonth) LastMonth();
			if (Operation == Operation.AppendLastWeekend) LastWeekend();
    		
    		foreach (var config in List) config.Execute(WorkingDirectory);
		}
		
		static public Configs LoadXml(string filePath)
		{
            XmlValidator.ValidateConfig(filePath);
            var configs = EntitySerializer.Deserialize<Configs>(filePath);
		    configs.WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(filePath));
		    return configs;
		}
		
		public void SaveXml(string filePath)
		{
			EntitySerializer.Serialize(this, filePath);
			XmlValidator.ValidateConfig(filePath);
		}

        public Configs(IEnumerable<Config> configs)
        {
            List = new List<Config>(configs);
        }

        public Configs()
        {
            List = new List<Config>();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Configs Clone()
        {
            return new Configs(List.Select(c => c.Clone()));
        }
        
        public void LastDay()
        {
        	var date = DateTime.Now.AddDays(-1).Date;
        	SetFrom(date);
        	SetTo(date);
        	SetAppend();
        }
        
        public void LastWeekend()
        {
        	var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        	var friday = sunday.AddDays(-2);
        	
			SetFrom(friday);
        	SetTo(sunday);
        	SetAppend();
        }
        
        public void LastMonth()
        {
        	var month = DateTime.Now.AddMonths(-1).Month;
        	var year = DateTime.Now.AddMonths(-1).Year;
        	var day = DateTime.DaysInMonth(year, month);
        	
        	var from = new DateTime(year, month, 1);
        	var to = new DateTime(year, month, day);
        	
        	SetFrom(from);
        	SetTo(to);
        	SetAppend();
        }
        
        public void SetFrom(DateTime date)
        {
        	ModifyConfigs(date, (c, d) => c.From = d);
        }
        
        public void SetTo(DateTime date)
        {
        	ModifyConfigs(date, (c, d) => c.To = d);
        }
        
        public void SetAppend(bool value = true)
        {       	
        	ModifyConfigs(value, (c, b) => c.Append = b);
        }
        
        public void SetSourcePath(string path)
        {
            ModifyConfigs(path, (c, s) => c.SourcePath = path);
        }

        public void ModifyConfigs<T>(T value, Action<Config, T> action)
        {
        	foreach (var config in List)
        		action(config, value);        	
        }

        public void Add(Config c)
        {
            List.Add(c);
        }
    }
}
