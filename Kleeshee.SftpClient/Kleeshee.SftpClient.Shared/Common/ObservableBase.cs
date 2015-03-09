using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Kleeshee.SftpClient.Common
{
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        protected readonly CoreDispatcher Dispatcher;

        public ObservableBase(CoreDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                await this.Invoke(() => this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        protected async Task Invoke(Action action)
        {
            if (this.Dispatcher == null)
            {
                action();
            }
            else
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
            }
        }
    }
}
