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
            Config = new Config();
            DataContext = Config;
        }    

        public Config Config { get; private set; }

        private void templateCheck_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            templateBrowse.Path = null;
        }
    }
}
