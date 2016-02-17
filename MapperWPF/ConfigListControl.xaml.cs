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
    /// Interaction logic for ConfigListControl.xaml
    /// </summary>
    public partial class ConfigListControl : UserControl
    {
        public ConfigListControl()
        {
            InitializeComponent();
        }

        public Configs Configs
        {
            get { return ToConfigList(); }
            set { FromConfigList(value); }
        }

        private void FromConfigList(Configs c)
        {
            throw new NotImplementedException();
        }

        private Configs ToConfigList()
        {
            throw new NotImplementedException();
        }
    }
}
