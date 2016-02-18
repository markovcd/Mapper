using System.Windows.Controls;
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
            DataContext = new ConfigViewModel();
        }    

        private void templateCheck_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            templateBrowse.Path = null;
        }
    }
}
