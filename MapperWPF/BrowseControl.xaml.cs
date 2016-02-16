using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack;

namespace MapperWPF
{
    public class BrowseStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString()
                        .Split(',')
                        .Select(v => (BrowseStyle) Enum.Parse(typeof (BrowseStyle), v, true))
                        .Aggregate((e1, e2) => e1 | e2);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

        public BrowseStyle BrowseStyle { get; set; }
        public string Title { get; set; }
        public string Filters { get; set; }

        public string Path
        {
            get { return FileText.Text; }
            set { FileText.Text = value; }
        }

        public static readonly DependencyProperty BrowseStyleProperty =
          DependencyProperty.Register("BrowseStyle", typeof(BrowseStyle), typeof(BrowseControl));

        public static readonly DependencyProperty TitleProperty =
          DependencyProperty.Register("Title", typeof(string), typeof(BrowseControl));

        public static readonly DependencyProperty FiltersProperty =
          DependencyProperty.Register("Filters", typeof(string), typeof(BrowseControl));

        public static readonly DependencyProperty PathProperty =
          DependencyProperty.Register("Path", typeof(string), typeof(BrowseControl));

        private IEnumerable<CommonFileDialogFilter> ParseFilters()
        {
            var split = Filters.Split('|');

            for (var i = 0; i < split.Length - 1; i += 2)
            {
                yield return new CommonFileDialogFilter(split[i], split[i + 1]);
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var d = CreateFileDialog();
            if (d.ShowDialog() == CommonFileDialogResult.Cancel) return;
            Path = d.FileName;
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
            foreach (var f in ParseFilters()) d.Filters.Add(f);

            return d;
        }
    }
}
