using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VSCCoreUWP.Views
{
    public class TextData : INotifyPropertyChanged
    {
        private string textData;
        private string textFileNameString = "untitled_text_file.txt";

        public string TextStringData
        {
            get
            {
                return textData;
            }
            set
            {
                textData = value;
                this.OnPropertyChanged();
            }
        }

        public string TextFileNameString
        {
            get
            {
                return textFileNameString;
            }
            set
            {
                textFileNameString = value;
                this.OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
