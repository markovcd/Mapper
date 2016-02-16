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

        public Config ToConfig()
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
    }
}
