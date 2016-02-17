using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack;

namespace MapperWPF
{
    [Flags]
    public enum BrowseStyle
    {
        File = 1, Directory = 2, Open = 4, Save = 8
    }

    /// <summary>
    /// Interaction logic for BrowseControl.xaml
    /// </summary>
    public partial class BrowseControl : UserControl
    {
        public BrowseControl()
        {
            InitializeComponent();
        }

        public BrowseStyle BrowseStyle
        {
            get { return (BrowseStyle) GetValue(BrowseStyleProperty); }
            set { SetValue(BrowseStyleProperty, value);}
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Filters
        {
            get { return (string)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty BrowseStyleProperty =
          DependencyProperty.Register("BrowseStyle", typeof(BrowseStyle), typeof(BrowseControl));

        public static readonly DependencyProperty TitleProperty =
          DependencyProperty.Register("Title", typeof(string), typeof(BrowseControl));

        public static readonly DependencyProperty FiltersProperty =
          DependencyProperty.Register("Filters", typeof(string), typeof(BrowseControl));

        public static readonly DependencyProperty PathProperty =
          DependencyProperty.Register("Path", typeof(string), typeof(BrowseControl));

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetValue(DependencyProperty property, object value,
            [System.Runtime.CompilerServices.CallerMemberName] string p = null)
        {
            base.SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        private IEnumerable<CommonFileDialogFilter> ParseFilters()
        {
            if (Filters == null) yield break;

            var split = Filters.Split('|');

            for (var i = 0; i < split.Length - 1; i += 2)
                yield return new CommonFileDialogFilter(split[i], split[i + 1]);
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            using (var d = CreateFileDialog())
            {
                if (d.ShowDialog() == CommonFileDialogResult.Cancel) return;
                Path = d.FileName;
            }
        }

        private CommonFileDialog CreateFileDialog()
        {
            var isDirectory = BrowseStyle.HasFlag(BrowseStyle.Directory);
            var isSave = BrowseStyle.HasFlag(BrowseStyle.Save);
            var isOpen = BrowseStyle.HasFlag(BrowseStyle.Open);

            if (isSave && isDirectory) throw new InvalidOperationException();
            CommonFileDialog d = null;

            if (isSave) d = new CommonSaveFileDialog
            {
                EnsureValidNames = true
            };
            if (isOpen) d = new CommonOpenFileDialog
            {
                IsFolderPicker = isDirectory,
                EnsureFileExists = true,

            };

            if (d == null) throw new InvalidOperationException();

            d.Title = Title;
            d.NavigateToShortcut = true;
            d.ShowPlacesList = true;

            if (!isDirectory)
                foreach (var f in ParseFilters()) d.Filters.Add(f);

            return d;
        }
    }
}
