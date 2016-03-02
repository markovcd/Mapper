using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;
using System.Windows.Threading;

namespace MapperWPF.Utilities
{
	/// <summary>
	/// A DataSourceProvider which exposes a collection ObservableFileInfo
	/// objects, representing the files in a particular directory.  Changes
	/// made to the files in that directory are reflected at runtime.
	/// </summary>
	public class FileSystemDataProvider : DataSourceProvider
	{
		#region Data

		private readonly ObservableFileInfoCollection _files;
		private readonly FileSystemWatcher _watcher;

		#endregion // Data

		#region Constructor

		public FileSystemDataProvider()
		{
			_files = new ObservableFileInfoCollection();

			_watcher = new FileSystemWatcher();

			_watcher.NotifyFilter =
				NotifyFilters.LastAccess |
				NotifyFilters.LastWrite |
				NotifyFilters.FileName;

			_watcher.Changed += this.OnFileChanged;
			_watcher.Created += this.OnFileCreated;
			_watcher.Deleted += this.OnFileDeleted;
			_watcher.Renamed += this.OnFileRenamed;
		}

		#endregion // Constructor

		#region Properties

		/// <summary>
		/// Gets/sets the directory whose files is monitored.
		/// </summary>
		public string Path
		{
			get { return _watcher.Path; }
			set
			{
				if (_watcher.Path == System.IO.Path.GetFullPath(value))
					return;

				if (!Directory.Exists(value))
				{
					Debug.Fail("Directory does not exist: " + value);
				}
				else
				{
					_watcher.Path = value;

					// Once the watcher has a valid path, then 
					// we can turn events on for it.
					if (!_watcher.EnableRaisingEvents)
						_watcher.EnableRaisingEvents = true;

					base.OnPropertyChanged(
						new PropertyChangedEventArgs("Path"));
				}
			}
		}
		
		public string Filter
		{
			get { return _watcher.Filter; }
			set
			{
				if (_watcher.Filter == value)
					return;

				
				_watcher.Filter = value;

				base.OnPropertyChanged(
					new PropertyChangedEventArgs("Filter"));
				
			}
		}

		#endregion // Properties

		#region BeginQuery

		/// <summary>
		/// Stores information about all of the files 
		/// in the target directory.
		/// </summary>
		protected override void BeginQuery()
		{
			string path = _watcher.Path;

			if (Directory.Exists(path))
			{
				if (0 < _files.Count)
					_files.Clear();

				string[] filePaths = Directory.GetFiles(path);

				foreach (string filePath in filePaths)
					_files.Add(new ObservableFileInfo(filePath));
			}

			// Hand off the results to the base class.
			base.OnQueryFinished(_files);
		}

		#endregion // BeginQuery

		#region FileSystemWatcher Event Handlers

		void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			if (!base.Dispatcher.CheckAccess())
			{
				base.Dispatcher.BeginInvoke(
					DispatcherPriority.Normal,
					(FileSystemEventHandler)OnFileChanged,
					sender, e);
			}
			else
			{				
				ObservableFileInfo changedFile = _files.GetFile(e.FullPath);
				if (changedFile != null)
				{
					Debug.WriteLine("File Changed: " + e.FullPath);

					changedFile.Refresh();
				}
			}
		}

		void OnFileCreated(object sender, FileSystemEventArgs e)
		{
			if (!base.Dispatcher.CheckAccess())
			{
				base.Dispatcher.BeginInvoke(
					DispatcherPriority.Normal,
					(FileSystemEventHandler)OnFileCreated,
					sender, e);
			}
			else
			{
				// Ignore new directories.
				if (File.Exists(e.FullPath))
				{
					Debug.WriteLine("File Created: " + e.FullPath);

					_files.Add(new ObservableFileInfo(e.FullPath));
				}
			}
		}

		void OnFileDeleted(object sender, FileSystemEventArgs e)
		{
			if (!base.Dispatcher.CheckAccess())
			{
				base.Dispatcher.BeginInvoke(
					DispatcherPriority.Normal,
					(FileSystemEventHandler)OnFileDeleted,
					sender, e);
			}
			else
			{			
				ObservableFileInfo deletedFile = _files.GetFile(e.FullPath);
				if (deletedFile != null)
				{
					Debug.WriteLine("File Deleted: " + e.FullPath);

					_files.Remove(deletedFile);
				}
			}
		}

		void OnFileRenamed(object sender, RenamedEventArgs e)
		{
			if (!base.Dispatcher.CheckAccess())
			{
				base.Dispatcher.BeginInvoke(
					DispatcherPriority.Normal,
					(RenamedEventHandler)OnFileRenamed,
					sender, e);
			}
			else
			{
				ObservableFileInfo file = _files.GetFile(e.OldFullPath);
				if (file != null)
				{
					Debug.WriteLine("File Renamed: " + e.FullPath);

					file.ChangeName(e.FullPath);
				}
			}
		}

		#endregion // FileSystemWatcher Event Handlers		
	}
}