using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Brenn_und_Plasmaschneidanlage.ViewModel
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T storage , T value , [CallerMemberName] string proberty = null)
        {
            if (Object.Equals(storage , value)) return;
            storage = value;
            PropertyChanged?.Invoke(this , new PropertyChangedEventArgs(proberty));
        }
    }
}
