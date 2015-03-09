using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Kleeshee.SftpClient.Common
{
    public class ObservableAsyncCollection<T> : IReadOnlyCollection<T>, IObservable<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IObservable<T> observable;

        private readonly ObservableCollection<T> collection = new ObservableCollection<T>();

        public ObservableAsyncCollection(IObservable<T> observable)
        {
            this.observable = observable;
            this.observable.Subscribe(item => this.collection.Add(item));
            this.collection.CollectionChanged += this.OnCollectionChanged;
            ((INotifyPropertyChanged)this.collection).PropertyChanged += this.OnPropertyChanged;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return this.observable.Subscribe(observer);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(sender, e);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(sender, e);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get { return this.collection.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }
    }
}
