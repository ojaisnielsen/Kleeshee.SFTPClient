using Kleeshee.SftpClient.DataModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Kleeshee.SftpClient.Common;
using Kleeshee.SftpClient.DataModels.Mocks;
using Windows.ApplicationModel.Resources;

namespace Kleeshee.SftpClient.ViewModels.Mocks
{
    public class FileBrowserViewModelMock : IFileBrowserViewModel
    {
        private readonly ObservableCollection<object> files = new ObservableCollection<object>();

        private readonly ObservableCollection<object> folders = new ObservableCollection<object>();

        public FileBrowserViewModelMock()
        {
            this.CurrentFolder = new SftpFolderMock { Name = "foldername", Path = "/path/to/parent/foldername" };
            var file1 = this.CurrentFolder.CreateFileAsync("file1.txt").Result;
            file1.ContentType = "text/plain";
            file1.Size = 1000000;
            var file2 = this.CurrentFolder.CreateFileAsync("file2.png").Result;
            file2.ContentType = "image/png";
            file2.Progress = 0.2;
            file2.Size = 500;
            var folder1 = this.CurrentFolder.CreateFolderAsync("folder1").Result;
            var folder2 = this.CurrentFolder.CreateFolderAsync("folder2").Result;
            this.DisplayPath = this.CurrentFolder.Path;
            this.AddressPath = this.CurrentFolder.Path;
            this.Host = "192.168.0.1";
            this.NewFolderName = "folder3";

            var loader = new ResourceLoader();
            var applicationId = loader.GetString("AdApplicationId1Debug");
            var adUnitId = loader.GetString("AdUnitId1Debug");
            this.files.Add(new AdItem { Id = 0, Serving = false });

            this.files.AddRange(this.CurrentFolder.GetItemsAsync().Result.OfType<ISftpFile>());
            this.folders.AddRange(this.CurrentFolder.GetItemsAsync().Result.OfType<ISftpFolder>());
        }

        public ObservableCollection<object> Files { get { return this.files; } }

        public ObservableCollection<object> Folders { get { return this.folders; } }

        public bool IsLoading { get; set; }

        public ISftpFolder CurrentFolder { get; set; }

        public bool IsLoaded
        {
            get { return !this.IsLoading; }
        }

        public string DisplayPath { get; set; }

        public string AddressPath { get; set; }

        public string NewFolderName { get; set; }

        public string Host { get; set; }

        public ListViewSelectionMode FileSelectionMode { get; set; }

        public bool AreFilesClickable { get; set; }

        public bool IsAddressBarVisible { get; set; }

        public async Task LoadPageAsync(string path)
        {
        }

        public async Task CreateFolderAsync()
        {
        }

        public async Task DownloadAsync(ISftpFile fileToDownload)
        {
        }

        public async Task UploadAsync()
        {
        }

        public async Task DeleteAsync(ISftpFile fileToDelete)
        {
        }

        public event EventHandler ShowSettings;
    }
}
