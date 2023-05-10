using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace VSCCoreUWP.Models
{
    public class SerialiseTextFiles
    {
        public string text = string.Empty;
        public string fileNameText = string.Empty;

        public readonly string FILE_NAMES = "TEXT_DATA_FILE_NAMES.txt"; 
        private MainPage mainPageRef;
        public IReadOnlyList<Windows.Storage.StorageFile> fileNameIList;

        public List<string> filePathList = new List<string>();
        public List<string> fileNameList = new List<string>();
        public List<string> filePasswordsList = new List<string>();

        public SerialiseTextFiles(MainPage mainPage)
        {
            mainPageRef = mainPage;
        }

        public async void SaveMetaData(string fileName)
        {
            // Store the text data file contents.

                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(fileName,
                Windows.Storage.CreationCollisionOption.OpenIfExists);

                fileNameList.Add(fileName);

            // Make sure this add function does not break the code.
            filePathList.Add(file.Path);


            // await Windows.Storage.FileIO.WriteLinesAsync(file, fileNameList);

            // this.mainPageRef.UpdateUI(); // The placement here is important. As 'async' suspends this SaveMetaData function, the inherent async abstraction only lets the UpdateUI() process once the 'await' line is complete.
                await Windows.Storage.FileIO.WriteTextAsync(file, mainPageRef.TextData.TextStringData);

            // Store the text data file name.
            //
            /*if (fileName.Substring(0, 10) == "FILE_NAME_") 
             {
               Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
               Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(FILE_NAMES, // this needs suitable protection.
               Windows.Storage.CreationCollisionOption.OpenIfExists);

               fileNameList.Add(fileName);
               await Windows.Storage.FileIO.WriteLinesAsync(file, fileNameList);
             }*/

            this.mainPageRef.UpdateUI(); // This function should be called last, as it refreshes the field information-- Don't call this before the await WrtieText().
        }

        public async void LoadMetaData(bool onInit, string fileName)
        {
            if (onInit == false)
            {
                try
                {
                    string name = fileName;
                    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                    for (int index = 0; index < this.filePathList.Count; index++)
                    {
                        // Get the full filepath of the file in focus.
                        if (this.filePathList[index].ToString().Contains(name))
                        {
                            //this.mainPageRef.UpdateUI_domainSelected(); // The placement here is important. As 'async' suspends this SaveMetaData function, the inherent async abstraction only lets the UpdateUI() process once the 'await' line is complete.
                            //Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(this.filePathList[index]); // pass the full file-path.

                            text = this.filePasswordsList[index];
                            fileNameText = name;
                        }
                    }

                    /*
                    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\" + fileName);

                    text = await Windows.Storage.FileIO.ReadTextAsync(file); */
                }
                catch
                {
                    /*
                    string name = fileName;
                    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(name);

                    text = await Windows.Storage.FileIO.ReadTextAsync(file);
                    fileNameText = name;*/
                }
            }
            else  // Load the data files upon program boot-up.
            {
                try
                {
                    string name = fileName;
                    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                    this.mainPageRef.UpdateUI_domainSelected(); // The placement here is important. As 'async' suspends this SaveMetaData function, the inherent async abstraction only lets the UpdateUI() process once the 'await' line is complete.
                    fileNameIList = await storageFolder.GetFilesAsync();
                    
                    // Populate a locally-stored list of file-names.
                    foreach (IStorageFile file in this.fileNameIList)
                    {
                        this.filePathList.Add(file.Path); // Store the filepath of the credential.
                        this.fileNameList.Add(file.Name); // Store the file-name, or "domain" of the credential.
                        this.filePasswordsList.Add(await Windows.Storage.FileIO.ReadTextAsync(file)); // Store the password of the credential.
                    }

                    // Re-construct the UI with the loaded credentials information.
                    this.mainPageRef.UpdateUI();
                }
                catch
                {
                    string name = fileName;
                    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(name);

                    text = await Windows.Storage.FileIO.ReadTextAsync(file);
                    fileNameText = name;
                }
            }
        }

        public static string ConstructTextFromFile(string filepath)
        {
            string textData = string.Empty;

            return textData = (string)DeserialiseTextFile<string>(filepath);
        }

        private static T DeserialiseTextFile<T>(string filepath) // where T : new() <-- this could be pretty useful.
        {
            TextReader reader = null;

            try
            {
                var serialiser = new XmlSerializer(typeof(T));
                reader = new StreamReader(filepath);
                return (T)serialiser.Deserialize(reader);

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}
