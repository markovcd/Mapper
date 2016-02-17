using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mapper;

namespace MapperWPF
{
    /// <summary>
    /// Interaction logic for ConfigControl.xaml
    /// </summary>
    public partial class ConfigControl : UserControl
    {
        public ConfigControl()
        {
            InitializeComponent();
        }

        private Config ToConfig()
        {
            return new Config
            {
                From = FromBox.Value.Value,
                To = ToBox.Value.Value,
                ConfigPath = ConfigPathBox.Path,
                SourcePath = SourcePathBox.Path,
                TemplatePath = templateCheck.IsChecked.Value ? TemplatePathBox.Path : null,
                TargetPath = TargetPathBox.Path,
                Append = AppendBox.IsChecked.Value
            };
        }

        public Config Config
        {
            get { return ToConfig(); }
            set { FromConfig(value); }
        }

        private void FromConfig(Config value)
        {
            FromBox.Value = value.From;
            ToBox.Value = value.To;
            ConfigPathBox.Path = value.ConfigPath;
            SourcePathBox.Path = value.SourcePath;
            TemplatePathBox.Path = value.TemplatePath;
            TargetPathBox.Path = value.TargetPath;
            AppendBox.IsChecked = value.Append;
            templateCheck.IsChecked = string.IsNullOrEmpty(value.TemplatePath);
        }
    }
}
