using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public interface IErrorReporter
    { void ReportError(string mes); }
}
