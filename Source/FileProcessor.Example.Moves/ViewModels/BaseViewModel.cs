//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Example.Moves
//    File Name: BaseViewModel.cs
//  
//    Author:    Andrew - 2017/05/31
//  ---------------------------------------------

namespace FileProcessor.Example.Moves.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}