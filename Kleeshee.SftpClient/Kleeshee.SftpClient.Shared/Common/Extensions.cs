using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Windows.Security.Credentials;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage.Streams;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

namespace Kleeshee.SftpClient.Common
{
    public static class Extensions
    {
        public static bool TryGetCredential(this PasswordVault vault, string resource, string username, out PasswordCredential credential)
        {
            var credentials = from cred in vault.RetrieveAll()
                              where (resource == null || cred.Resource == resource) && (username == null || cred.UserName == username)
                              select cred;
            var tempCredential = credentials.FirstOrDefault();
            credential = tempCredential;
            return tempCredential != null;
        }

        public static void RemoveAll(this PasswordVault vault, string resource, string username)
        {
            var credentials = from cred in vault.RetrieveAll()
                              where (resource == null || cred.Resource == resource) && (username == null || cred.UserName == username)
                              select cred;
            foreach (var credential in credentials)
            {
                vault.Remove(credential);
            }
        }

        public static Stream AsUtf8Stream(this string text)
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        }

        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
        {
            return new ReadOnlyList<T>(list);
        }

        public static IRandomAccessStreamWithContentType AsRandomAccessStreamWithContentType(this IRandomAccessStream randomAccessStream, string contentType)
        {
            return new RandomAccessStreamWithContentType(randomAccessStream, contentType);
        }

        public static ObservableAsyncCollection<T> ToObservableCollection<T>(this IObservable<T> observable)
        {
            return new ObservableAsyncCollection<T>(observable);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                collection.Add(item);
            }
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                collection.Remove(item);
            }
        }

        public static T FirstParentOfTypeOrDefault<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            while (dependencyObject != null && !(dependencyObject is T))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            return (T)dependencyObject;
        }
    }
}
