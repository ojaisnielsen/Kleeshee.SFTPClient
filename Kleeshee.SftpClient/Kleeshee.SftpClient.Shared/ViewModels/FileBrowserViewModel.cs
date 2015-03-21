using Kleeshee.SftpClient.Common;
using Kleeshee.SftpClient.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Kleeshee.SftpClient.ViewModels
{
    public class FileBrowserViewModel : ObservableBase, IFileBrowserViewModel
    {
        private readonly ObservableCollection<object> files = new ObservableCollection<object>();

        private readonly ObservableCollection<object> folders = new ObservableCollection<object>();

        private readonly SftpDataSource DataSource;

        private bool isLoading;

        private bool isAddressBarVisible;

        private string host;

        private string displayPath;

        private string addressPath;

        private string newFolderName;

        private ISftpFolder currentFolder;

        private ListViewSelectionMode fileSelectionMode;

        private bool areFilesClickable;

        public FileBrowserViewModel(SftpDataSource dataSource, CoreDispatcher dispatcher, bool showAds) : base(dispatcher)
        {
            this.DataSource = dataSource;
            this.FileSelectionMode = ListViewSelectionMode.None;
            this.AreFilesClickable = true;
            this.Host = "Kleeshee SFTP Client";
            if (showAds)
            {
#if DEBUG
                var fileAd = new AdItem { Id = 0, Serving = false };
                var folderAd = new AdItem { Id = 1, Serving = false };
#else
                var fileAd = new AdItem { Id = 0, Serving = true };
                var folderAd = new AdItem { Id = 1, Serving = true };
#endif
                this.Files.Add(fileAd);
                this.Folders.Add(folderAd);
            }
        }

        public ObservableCollection<object> Files { get { return this.files; } }

        public ObservableCollection<object> Folders { get { return this.folders; } }

        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.isLoading = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("IsLoaded");
            }
        }

        public ISftpFolder CurrentFolder
        {
            get
            {
                return this.currentFolder;
            }

            set
            {
                this.currentFolder = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsLoaded { get { return !this.IsLoading; } }

        public string DisplayPath
        {
            get
            {
                return this.displayPath;
            }

            set
            {
                this.displayPath = value;
                this.OnPropertyChanged();
            }
        }

        public string AddressPath
        {
            get
            {
                return this.addressPath;
            }

            set
            {
                this.addressPath = value;
                this.OnPropertyChanged();
                this.DisplayPath = value;
            }
        }

        public string NewFolderName
        {
            get
            {
                return this.newFolderName;
            }

            set
            {
                this.newFolderName = value;
                this.OnPropertyChanged();
            }
        }

        public string Host
        {
            get
            {
                return this.host;
            }

            set
            {
                this.host = value;
                this.OnPropertyChanged();
            }
        }

        public ListViewSelectionMode FileSelectionMode
        {
            get
            {
                return this.fileSelectionMode;
            }

            set
            {
                this.fileSelectionMode = value;
                this.OnPropertyChanged();
            }
        }

        public bool AreFilesClickable
        {
            get
            {
                return this.areFilesClickable;
            }

            set
            {
                this.areFilesClickable = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsAddressBarVisible
        {
            get
            {
                return this.isAddressBarVisible;
            }

            set
            {
                this.isAddressBarVisible = value;
                this.OnPropertyChanged();
            }
        }

        public async Task LoadPageAsync(string path)
        {
            var dialog = default(MessageDialog);
            try
            {
                path = string.IsNullOrWhiteSpace(path) ? "." : path;
                this.AddressPath = path;
                this.DisplayPath = "...";
                this.IsLoading = true;
                this.Host = string.Format("{0}@{1}", this.DataSource.ConnectionInfo.Username, this.DataSource.ConnectionInfo.Host);

                if (!this.DataSource.IsConnected)
                {
                    await this.DataSource.ConnectAsync();
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);

                dialog = new MessageDialog("Please provide valid SSH settings and make sure your host is reachable.");
                dialog.Commands.Add(new UICommand("OK", new UICommandInvokedHandler((command) => this.OnShowSettings())));
                dialog.Commands.Add(new UICommand("Not now", null));
                this.DataSource.Dispose();
            }

            if (dialog != null)
            {
                this.IsLoading = false;
                this.OnAskReconnect(dialog);
                return;
            }

            try
            {
                this.CurrentFolder = await this.DataSource.GetFolderAsync(this.AddressPath, this.Dispatcher);
                this.AddressPath = this.CurrentFolder.Path;
                var items = await this.CurrentFolder.GetItemsAsync();
                var files = items.OfType<SftpFile>();
                this.ResetFiles();
                this.Files.AddRange(files);
                var folders = from folder in items.OfType<SftpFolder>()
                              where folder.Name != "." && folder.Name != ".."
                              select folder;
                this.ResetFolders();
                this.Folders.AddRange(folders);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
                dialog = new MessageDialog("Something went wrong, please try again in a bit.");
                this.DataSource.Dispose();
            }

            this.IsLoading = false;

            if (dialog != null)
            {
                await dialog.ShowAsync();
            }
        }

        public async Task CreateFolderAsync()
        {
            var dialog = default(MessageDialog);
            try
            {
                if (string.IsNullOrWhiteSpace(this.NewFolderName))
                {
                    return;
                }

                var newFolder = await this.CurrentFolder.CreateFolderAsync(this.NewFolderName);
                this.Folders.Add(newFolder);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
                dialog = new MessageDialog("Something went wrong, please try again in a bit.");
            }

            if (dialog != null)
            {
                await dialog.ShowAsync();
            }
        }

        public async Task DownloadAsync(ISftpFile fileToDownload)
        {
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.Downloads;
            fileSavePicker.FileTypeChoices.Add("Any file type", new List<string>() { "." });
            fileSavePicker.SuggestedFileName = fileToDownload.Name;
            var fileToSave = await fileSavePicker.PickSaveFileAsync();
            if (fileToSave == null)
            {
                return;
            }

            var startNotification = NotificationHelper.Factory("Download started", string.Format("The file {0} has started downloading...", fileToDownload.Name), fileToDownload.ImagePath);
            ToastNotificationManager.CreateToastNotifier().Show(startNotification);

            try
            {
                using (var targetStream = await fileToSave.OpenStreamForWriteAsync())
                {
                    var download = fileToDownload.DownloadAsyncWithProgress(targetStream);
                    download.Progress = (info, progress) => { fileToDownload.Progress = progress / (double)fileToDownload.Size; };
                    await download;
                }

                var successNotification = NotificationHelper.Factory("Download finished", string.Format("The file {0} was downloaded successfully.", fileToDownload.Name), fileToDownload.ImagePath);
                ToastNotificationManager.CreateToastNotifier().Show(successNotification);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);

                var errorNotification = NotificationHelper.Factory("Download failed", string.Format("The file {0} could not be downloaded.", fileToDownload.Name), fileToDownload.ImagePath);
                ToastNotificationManager.CreateToastNotifier().Show(errorNotification);
            }
        }


        public async Task UploadAsync()
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add("*");
            filePicker.ViewMode = PickerViewMode.List;
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            var fileToUpload = await filePicker.PickSingleFileAsync();
            if (fileToUpload == null)
            {
                return;
            }

            var startNotification = NotificationHelper.Factory("Upload started", string.Format("The file {0} has started uploading...", fileToUpload.Name));
            ToastNotificationManager.CreateToastNotifier().Show(startNotification);

            var uploadedFile = default(ISftpFile);
            try
            {
                uploadedFile = await this.CurrentFolder.CreateFileAsync(fileToUpload.Name, CreationCollisionOption.GenerateUniqueName);
                this.Files.Add(uploadedFile);
                var sourceProperties = await fileToUpload.GetBasicPropertiesAsync();
                using (var input = await fileToUpload.OpenStreamForReadAsync())
                {
                    var upload = uploadedFile.CopyAndReplaceAsyncWithProgress(input);
                    upload.Progress = (info, progress) => 
                    {
                        uploadedFile.Size = progress;
                        uploadedFile.Progress = progress / (double)sourceProperties.Size; 
                    };
                    await upload;
                }

                var successNotification = NotificationHelper.Factory("Upload finished", string.Format("The file {0} was uploaded successfully.", fileToUpload.Name), uploadedFile.ImagePath);
                ToastNotificationManager.CreateToastNotifier().Show(successNotification);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);

                var errorNotification = NotificationHelper.Factory("Upload failed", string.Format("The file {0} could not be uploaded.", fileToUpload.Name));
                ToastNotificationManager.CreateToastNotifier().Show(errorNotification);
            }
        }

        public async Task DeleteAsync(ISftpFile fileToDelete)
        {
            var confirmationDialog = new MessageDialog("Are you sure?");
            confirmationDialog.Commands.Add(new UICommand("Yes", null, true));
            confirmationDialog.Commands.Add(new UICommand("No", null, false));
            var confirmation = await confirmationDialog.ShowAsync();
            if (!(bool)confirmation.Id)
            {
                return;
            }

            var dialog = default(MessageDialog);
            try
            {
                await fileToDelete.DeleteAsync();
                this.Files.Remove(fileToDelete);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);

                dialog = new MessageDialog("Something went wrong, please try again in a bit.");
            }

            if (dialog != null)
            {
                await dialog.ShowAsync();
            }
        }

        public event EventHandler ShowSettings;

        public event EventHandler<MessageDialog> AskReconnect;

        private async void OnShowSettings()
        {
            if (this.ShowSettings != null)
            {
                await this.Invoke(() => this.ShowSettings(this, new EventArgs()));
            }
        }

        private async void OnAskReconnect(MessageDialog dialog)
        {
            if (this.AskReconnect != null)
            {
                await this.Invoke(() => this.AskReconnect(this, dialog));
            }
        }

        private void ResetFiles()
        {
            var actualFiles = this.Files.OfType<ISftpFile>().ToList();
            this.Files.RemoveRange(actualFiles);
        }

        private void ResetFolders()
        {
            var actualFolders = this.Files.OfType<ISftpFolder>().ToList();
            this.Folders.RemoveRange(actualFolders);
        }
    }
}
