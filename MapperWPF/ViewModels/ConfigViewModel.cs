using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Mapper;
using MapperWPF.Utilities;

namespace MapperWPF.ViewModels
{
    public class ConfigViewModel : ObservableObject, ICloneable
    {
        public ConfigViewModel(Config config = null)
        {
            Config = config ?? new Config();

            ConfigPaths = new ObservableCollection<string>(Directory
                .EnumerateFiles("Mappings", "*.xml", SearchOption.AllDirectories));
        }

        public Config Config { get; private set; }

        public string Name
        {
            get { return Config.Name; }
            set { Config.Name = value; OnPropertyChanged("Name"); }
        }

        public DateTime? From
        {
            get { return Config.From.Equals(DateTime.MinValue) ? null : (DateTime?)Config.From; }
            set
            {
                Config.From = value ?? DateTime.MinValue;
                OnPropertyChanged("From");
            }
        }

        public DateTime? To
        {
            get { return Config.To.Equals(DateTime.MinValue) ? null : (DateTime?)Config.To; }
            set
            {
                Config.To = value ?? DateTime.MinValue;
                OnPropertyChanged("To");
            }
        }

        public ObservableCollection<string> ConfigPaths { get; private set; } 

        public string ConfigPath
        {
            get { return Config.ConfigPath; }
            set { Config.ConfigPath = value; OnPropertyChanged("ConfigPath"); }
        }

        public string SourcePath
        {
            get { return Config.SourcePath; }
            set { Config.SourcePath = value; OnPropertyChanged("SourcePath"); }
        }

        public string TemplatePath
        {
            get { return Config.TemplatePath; }
            set { Config.TemplatePath = value; OnPropertyChanged("TemplatePath"); }
        }

        public string TargetPath
        {
            get { return Config.TargetPath; }
            set { Config.TargetPath = value; OnPropertyChanged("TargetPath"); }
        }

        public bool Append
        {
            get { return Config.Append; }
            set { Config.Append = value; OnPropertyChanged("Append"); }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public ConfigViewModel Clone()
        {
            return new ConfigViewModel {Config = Config.Clone()};
        }

        public override string ToString()
        {
            return ConfigPath;
        }
    }
}
