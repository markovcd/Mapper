using System.Windows;
using System.Windows.Controls;
using Mapper;
using MapperWPF.ViewModels;

namespace MapperWPF.Controls
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

        private void templateCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            templateBrowse.Path = null;
        }
    }
}
