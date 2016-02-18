using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Mapper;

namespace MapperWPF.ViewModels
{
    class ConfigsViewModel
    {
        public ObservableCollection<ConfigViewModel> Configs { get; private set; }

        public Configs ToEntity()
        {
            return new Configs(Configs.Select(c => c.Config));
        }

        public ConfigsViewModel()
        {
            Configs = new ObservableCollection<ConfigViewModel>();
        }
    }
}
