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
using MapperWPF.ViewModels;
using WPF.JoshSmith.ServiceProviders.UI;

namespace MapperWPF.Controls
{
    /// <summary>
    /// Interaction logic for ConfigListControl.xaml
    /// </summary>
    public partial class ConfigsControl : UserControl
    {
        public ConfigsControl()
        {
            InitializeComponent();
            list.Tag = new ListViewDragDropManager<ConfigViewModel>(list);
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
