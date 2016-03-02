using System;
using System.ComponentModel;
using System.IO;

namespace MapperWPF.Utilities
{
	/// <summary>
	/// Exposes a FileInfo object and raises 
	/// a property changed notification event
	/// when the file is modified.
	/// </summary>
	public class ObservableFileInfo : ObservableObject
	{
		#region Data

		private FileInfo _fileInfo;
		private string _fileName;
		private bool _isChanged;

		#endregion // Data

		#region Constructor

		public ObservableFileInfo(string file)
		{
			_fileName = file;
			this.TakeSnapshot();
		}

		#endregion // Constructor

		#region Public Methods

		/// <summary>
		/// This method is called when the file 
		/// name is modified.
		/// </summary>
		/// <param name="newName">The new file name.</param>
		public void ChangeName(string newName)
		{
			_fileName = newName;
			this.Refresh();
		}

		/// <summary>
		/// This method is called when the file
		/// has been changed in some way.
		/// </summary>
		public void Refresh()
		{
			this.TakeSnapshot();
			this.MarkAsChanged();
		}

		#endregion // Public Methods

		#region Public Properties
		
		/// <summary>
		/// Returns information about the file.
		/// </summary>
		public FileInfo FileInfo
		{
			get { return _fileInfo; }
			private set
			{
				if (_fileInfo == value)
					return;

				_fileInfo = value;

				base.RaisePropertyChanged("FileInfo");
			}
		}

		// Used for styling purposes.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsChanged
		{
			get { return _isChanged; }
			private set
			{
				if (_isChanged == value)
					return;

				_isChanged = value;

				base.RaisePropertyChanged("IsChanged");
			}
		}
		
		#endregion // Public Properties

		#region Private Helpers

		private void MarkAsChanged()
		{
			// Set IsChanged to true so that
			// it will be noticed by the trigger
			// and the animation can start.  Then
			// immediately set it back to false.
			this.IsChanged = true;
			this.IsChanged = false;
		}

		private void TakeSnapshot()
		{
			this.FileInfo = new FileInfo(_fileName);	
		}

		#endregion // Private Helpers
	}
}