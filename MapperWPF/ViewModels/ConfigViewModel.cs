using System;
using Mapper;

namespace MapperWPF
{
    
    public class ConfigViewModel : ObservableObject
    {
        public ConfigViewModel()
        {
            Config = new Config();
        }

        public Config Config { get; private set; }

        public DateTime From
        {
            get { return Config.From; }
            set { Config.From = value; OnPropertyChanged("From"); }
        }

        public DateTime To
        {
            get { return Config.To; }
            set { Config.To = value; OnPropertyChanged("To"); }
        }

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
    }
}
