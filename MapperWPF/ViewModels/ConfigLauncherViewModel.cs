using System;
using MapperWPF.Utilities;

namespace MapperWPF.ViewModels
{
	/// <summary>
	/// Description of ConfigLauncherViewModel.
	/// </summary>
	public class ConfigLauncherViewModel : ObservableObject
	{
		public FileSystemDataProvider FileSystemDataProvider { get; set; }
		
		public ConfigLauncherViewModel()
		{
			FileSystemDataProvider = new FileSystemDataProvider();
		}
	}
}
