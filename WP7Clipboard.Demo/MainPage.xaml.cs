using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using WP7Clipboard;

namespace WP7Clipboard.Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string IsoStoreFileName = "wp7clipboard.demo.txt";

        private ProxyClipboard clpbrd;

        private string loadedContent;

        private bool shiftSelected;

        private int shiftSelectPositionStart;

        private int shiftSelectPositionLength;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.clpbrd = Clipboard.GenerateProxy();

            LoadTextFromIsolatedStorage();

            base.OnNavigatedTo(e);
        }

        private void LoadTextFromIsolatedStorage()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(IsoStoreFileName))
                {
                    using (var isfs = new IsolatedStorageFileStream(IsoStoreFileName, FileMode.Open, store))
                    {
                        using (var sr = new StreamReader(isfs))
                        {
                            loadedContent = sr.ReadToEnd();

                            tb.Text = loadedContent;
                        }
                    }
                }
                else
                {
                    // Load a custom message on first load
                    var sb = new StringBuilder();

                    sb.AppendLine("Thanks for looking at WP7Clipboard.");
                    sb.AppendLine("This demo project shows usage of the library, not best coding practices ;)");

                    tb.Text = sb.ToString();
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.clpbrd.Persist();

            SaveTextToIsolatedStorage();

            base.OnNavigatedFrom(e);
        }

        private void SaveTextToIsolatedStorage()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var isfs = new IsolatedStorageFileStream(IsoStoreFileName, FileMode.Create, store))
                {
                    using (var sw = new StreamWriter(isfs))
                    {
                        sw.Write(tb.Text);
                        sw.Close();
                    }
                }
            }
        }

        private void ShiftClick(object sender, EventArgs e)
        {
            shiftSelected = !shiftSelected;

            shiftSelectPositionStart = tb.SelectionStart;
            shiftSelectPositionLength = tb.SelectionLength;
        }

        private void CutClick(object sender, EventArgs e)
        {
            this.clpbrd.SetText(tb.SelectedText);

            var removeAt = tb.SelectionStart;

            var afterCut = tb.Text.Substring(removeAt + tb.SelectionLength);

            var beforeCut = tb.Text.Substring(0, removeAt);

            tb.Text = beforeCut + afterCut;

            tb.SelectionStart = beforeCut.Length;
        }

        private void CopyClick(object sender, EventArgs e)
        {
            this.clpbrd.SetText(tb.SelectedText);
        }

        private void PasteClick(object sender, EventArgs e)
        {
            var insertAt = tb.SelectionStart;

            var fromClipboard = this.clpbrd.GetText();

            tb.Text = tb.Text.Substring(0, insertAt) +
                      fromClipboard +
                      tb.Text.Substring(tb.SelectionStart + tb.SelectionLength);

            tb.SelectionStart = insertAt + fromClipboard.Length;
            tb.SelectionLength = 0;
        }

        private void tb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var currentStart = tb.SelectionStart;
            var currentLength = tb.SelectionLength;

            if (shiftSelected)
            {
                if (shiftSelectPositionStart < currentStart)
                {
                    tb.SelectionStart = shiftSelectPositionStart;
                    tb.SelectionLength = currentStart - shiftSelectPositionStart + currentLength;
                }
                else
                {
                    tb.SelectionStart = currentStart;
                    tb.SelectionLength = shiftSelectPositionStart - currentStart + shiftSelectPositionLength;
                }
            }

            shiftSelected = false;
        }
    }
}