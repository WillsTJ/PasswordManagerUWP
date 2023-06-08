using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VSCCoreUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Views.TextData TextData = new Views.TextData();
        public Models.SerialiseTextFiles serialiseText;

        public MainPage()
        {
            // I must make sure that all saved text data has their file names stored
            // by the program and loaded on each session. This is to circumvent a hurdle
            // of file-access restrictions/accessing specific directories.
            this.InitializeComponent();

            this.FileNamesListbox.SelectionChanged += new SelectionChangedEventHandler(this.FileNamesListbox_selectionChanged);
            serialiseText = new Models.SerialiseTextFiles(this);
            this.serialiseText.LoadMetaData(true, string.Empty);

            // Populate the list box visual with the data text file-names in source control.
            for (int index = 0; index < serialiseText.fileNameList.Count; index++)
            {
                this.FileNamesListbox.Items.Add(serialiseText.fileNameIList[index]);
            }
        }

        private async void FileNamesListbox_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Load a text file from discrete storage. (May need validation to ensure a file is selected).
                serialiseText.LoadMetaData(false, this.FileNamesListbox.SelectedItem.ToString());

                // Store the loaded text data in memory. Binding also takes care of the visual.
                TextData.TextStringData = serialiseText.text;
                TextData.TextFileNameString = serialiseText.fileNameText;

                // Refresh text buffer.
                serialiseText.text = string.Empty;
            }
            catch
            {
                //MessageDialog dialog = new MessageDialog("Please select a domain, to load the password.", "Load error");
                //await dialog.ShowAsync();
            }
        }

        private void UploadTextFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Populate the list box visual with the data text file-names.
            for (int index = 0; index < serialiseText.fileNameList.Count; index++)
            {
                this.FileNamesListbox.Items.Add(serialiseText.fileNameList[index]);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = this.TextFileMonitorHeaderTextBox.Text + ".txt";
            
            // Disable this event handler, to avoid conflicting erros when saving data.
            this.FileNamesListbox.SelectionChanged -= new SelectionChangedEventHandler(FileNamesListbox_selectionChanged);

            // Store the data-contents.
            serialiseText.SaveMetaData(fileName, this.JobListingDetailsTextBlock.Text);

            // Refresh the UI displaying the stored domain data.
            this.FileNamesListbox.Items.Clear();
            this.serialiseText.LoadMetaData(false, string.Empty);

            // Restore the event handler for the list box.
            this.FileNamesListbox.SelectionChanged += new SelectionChangedEventHandler(FileNamesListbox_selectionChanged);
        }

        public void UpdateUI_domainSelected()
        {
            try
            {
                // Store the loaded text data in memory. Binding also takes care of the visual.
                TextData.TextStringData = serialiseText.text;
                TextData.TextFileNameString = serialiseText.fileNameText;

                // Refresh text buffer.
                serialiseText.text = string.Empty;
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("Please select a domain, to load the password.", "Load error");
                dialog.ShowAsync();
            }
        }

        public void UpdateUI()
        {
            for (int index = 0; index < serialiseText.fileNameList.Count; index++)
            {
                this.FileNamesListbox.Items.Add(serialiseText.fileNameList[index]); // Update the recently-added file name for the UI.
            }

            // this.FileNamesListbox.Items[0] = this.serialiseText.fileNameList);
            this.FileNamesListbox.InvalidateMeasure();

            // Refresh the fields.
            this.TextFileMonitorHeaderTextBox.Text = string.Empty;
            this.JobListingDetailsTextBlock.Text = string.Empty; // Change these textbox control names.
        }
    }
}
