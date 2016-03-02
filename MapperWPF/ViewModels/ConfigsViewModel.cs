using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Mapper;
using MapperWPF.Utilities;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack;

namespace MapperWPF.ViewModels
{
	class ConfigsViewModel : ObservableObject
    {
        private ConfigViewModel currentConfigViewModel;
        private ObservableCollection<ConfigViewModel> configViewModels;

        public ObservableCollection<ConfigViewModel> ConfigViewModels
        {
            get { return configViewModels; }
            set { configViewModels = value; RaisePropertyChanged("ConfigViewModels"); }
        }
        
        public ConfigViewModel CurrentConfigViewModel
	    {
	        get { return currentConfigViewModel; }
            set { currentConfigViewModel = value; RaisePropertyChanged("CurrentConfigViewModel"); }
	    }

        public RelayCommand MoveUpCommand { get; private set; }
        public RelayCommand MoveDownCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand LoadCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public ConfigsViewModel()
        {
            ConfigViewModels = new ObservableCollection<ConfigViewModel>();
            MoveUpCommand = new RelayCommand(o => Move(-1), o => CanMove(-1));
            MoveDownCommand = new RelayCommand(o => Move(1), o => CanMove(1));
            RemoveCommand = new RelayCommand(o => Remove(), o => CanRemove());
            AddCommand = new RelayCommand(o => Add());
            LoadCommand = new RelayCommand(o => Load());
            SaveCommand = new RelayCommand(o => Save(), o => CanSave());
        }
        
	    public void Move(int i)
	    {
            var index = GetCurrentIndex();
            ConfigViewModels.Move(index, index + i);
        }

	    public bool CanMove(int i)
	    {
            var index = GetCurrentIndex();
            if (index == -1) return false;
            if (index + i >= ConfigViewModels.Count || index + i < 0) return false;
	        return true;
	    }

	    public void Remove()
	    {
	        ConfigViewModels.Remove(CurrentConfigViewModel);
	    }

	    public bool CanRemove()
	    {
	        return CurrentConfigViewModel != null;
	    }

	    public void Add()
	    {
	        var index = GetCurrentIndex();

	        var configViewModel = new ConfigViewModel();

            if (index == -1)
                ConfigViewModels.Add(configViewModel);
            else
                ConfigViewModels.Insert(index + 1, configViewModel);

	        CurrentConfigViewModel = configViewModel;
	    }

	    public void Load()
	    {
	        var d = new CommonOpenFileDialog {EnsureFileExists = true};
            d.Filters.Add(new CommonFileDialogFilter("Pliki xml", "*.xml"));
            d.Filters.Add(new CommonFileDialogFilter("Wszystkie pliki", "*.*"));
	        if (d.ShowDialog() == CommonFileDialogResult.Cancel) return;
	        Load(d.FileName);
	    }

	    public void Save()
	    {
	        var d = new CommonSaveFileDialog
            {
                EnsureValidNames = true,
                AlwaysAppendDefaultExtension = true,
                DefaultExtension = ".xml"
            };

            d.Filters.Add(new CommonFileDialogFilter("Pliki xml", "*.xml"));
            d.Filters.Add(new CommonFileDialogFilter("Wszystkie pliki", "*.*"));
            if (d.ShowDialog() == CommonFileDialogResult.Cancel) return;
            Save(d.FileName);
        }

	    public bool CanSave()
	    {
	        return ConfigViewModels != null && ConfigViewModels.Any();
	    }

	    public void Load(string filePath)
	    {
            ConfigViewModels = new ObservableCollection<ConfigViewModel>(Configs.LoadXml(filePath).List.Select(c => new ConfigViewModel(c)));
        }

	    public void Save(string filePath)
	    {
            new Configs(ConfigViewModels.Select(c => c.Config)).SaveXml(filePath);
        }

	    private int GetCurrentIndex()
	    {
	        return ConfigViewModels.IndexOf(currentConfigViewModel);
	    }
    }
}
