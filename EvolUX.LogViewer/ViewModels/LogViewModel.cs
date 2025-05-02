using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EvolUX.LogViewer.ViewModels
{
    public class LogListViewModel : INotifyPropertyChanged
    {
        // Implementation would go here
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LogDetailViewModel : INotifyPropertyChanged
    {
        // Implementation would go here
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}