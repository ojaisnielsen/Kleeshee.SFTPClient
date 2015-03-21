using Kleeshee.SftpClient.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Kleeshee.SftpClient.ViewModels
{
    public interface IFileBrowserViewModel
    {
        ObservableCollection<object> Files { get; }

        ObservableCollection<object> Folders { get; }

        bool IsLoading { get; set;}

        ISftpFolder CurrentFolder { get; set;}

        bool IsLoaded { get; }

        string DisplayPath { get; set;}

        string AddressPath { get; set;}

        string NewFolderName { get; set;}

        string Host { get; set;}

        ListViewSelectionMode FileSelectionMode { get; set;}

        bool AreFilesClickable { get; set; }

        bool IsAddressBarVisible { get; set; }

        Task LoadPageAsync(string path);

        Task CreateFolderAsync();

        Task DownloadAsync(ISftpFile fileToDownload);

        Task UploadAsync();

        Task DeleteAsync(ISftpFile fileToDelete);

        event EventHandler ShowSettings;

        event EventHandler<MessageDialog> AskReconnect;
    }
}
