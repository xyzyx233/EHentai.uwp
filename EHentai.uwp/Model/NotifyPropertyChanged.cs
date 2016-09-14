using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentai.uwp.Model
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {  }
        }

        protected void SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            try
            {
                if (Equals(storage, value)) return;

                storage = value;
                OnPropertyChanged(propertyName);
            }
            catch (Exception ex)
            { }
        }

    }
}
