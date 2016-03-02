using System;
using System.Collections.ObjectModel;

namespace MapperWPF.Utilities
{
	public class ObservableFileInfoCollection : ObservableCollection<ObservableFileInfo>
	{
		public ObservableFileInfo GetFile(string fullName)
		{
			ObservableFileInfo targetFile = null;

			foreach (ObservableFileInfo file in this)
			{
				if (file.FileInfo.FullName == fullName)
				{
					targetFile = file;
					break;
				}
			}

			return targetFile;
		}
	}
}