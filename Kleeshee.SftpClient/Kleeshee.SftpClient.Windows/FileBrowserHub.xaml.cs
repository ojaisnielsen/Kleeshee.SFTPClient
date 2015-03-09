using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Kleeshee.SftpClient.Common;
using Kleeshee.SftpClient.DataModels;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Kleeshee.SftpClient.ViewModels;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers.Provider;
using Windows.ApplicationModel.Activation;
namespace Kleeshee.SftpClient
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class FileBrowserHub : Page
    {
        private readonly NavigationHelper navigationHelper;
        //private FileOpenPickerUI fileOpenPickerUI;
        private readonly IFileBrowserViewModel viewModel;

        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper { get { return this.navigationHelper; } }

        public IFileBrowserViewModel ViewModel { get { return this.viewModel; } }

        public FileBrowserHub()
        {
            this.viewModel = new FileBrowserViewModel(((App)Application.Current).DataSource, this.Dispatcher, true);
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.OnLoadState;
            ((App)Application.Current).DataSource.Reseted += this.OnDataSourceReseted;
            this.ViewModel.ShowSettings += OnShowSettings;
        }

        void OnShowSettings(object sender, EventArgs e)
        {
            var settingsFlyout = new SshSettingsFlyout();
            settingsFlyout.Show();
        }

        //public async void ActivateFilePicker(FileOpenPickerActivatedEventArgs e)
        //{
        //    this.fileOpenPickerUI = e.FileOpenPickerUI;
        //    this.fileOpenPickerUI.FileRemoved += this.fileOpenPickerUI_FileRemoved;

        //    Window.Current.Content = this;
        //    Window.Current.Activate();

        //    this.ViewModel.FileSelectionMode = ListViewSelectionMode.Extended;
        //    this.ViewModel.AreFilesClickable = false;
        //    await this.ViewModel.LoadPageAsync(null);
        //}

        void fileOpenPickerUI_FileRemoved(FileOpenPickerUI sender, FileRemovedEventArgs args)
        {
            throw new NotImplementedException();
        }

        void OnDataSourceReseted(object sender, EventArgs e)
        {
            this.Frame.Navigate(typeof(FileBrowserHub), this.ViewModel.AddressPath);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void OnLoadState(object sender, LoadStateEventArgs e)
        {
            var path = e.NavigationParameter as string;
            await this.ViewModel.LoadPageAsync(path);
        }

        private void OnAddressKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                this.Frame.Navigate(typeof(FileBrowserHub), this.ViewModel.AddressPath);
            }
        }

        void OnAddressGoClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FileBrowserHub), this.ViewModel.AddressPath);
        }

        void OnGoUpClick(object sender, RoutedEventArgs e)
        {
            var path = DataModels.Common.CombinePaths(this.ViewModel.CurrentFolder.Path, "..");
            this.Frame.Navigate(typeof(FileBrowserHub), path);
        }

        void OnTitleClick(object sender, RoutedEventArgs e)
        {
            var settingsFlyout = new SshSettingsFlyout();
            settingsFlyout.Show();
        }

        void OnSubTitleClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.IsAddressBarVisible = true;
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void OnFileClick(object sender, ItemClickEventArgs e)
        {
            var gridView = (GridView)sender;
            var gridViewItem = (GridViewItem)gridView.ContainerFromItem(e.ClickedItem);
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)gridViewItem.ContentTemplateRoot); 
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void OnFolderClick(object sender, ItemClickEventArgs e)
        {
            var folder = (SftpFolder)e.ClickedItem;
            this.Frame.Navigate(typeof(FileBrowserHub), folder.Path);
        }

        async void OnFileMenuDownloadClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuFlyoutItem)sender;
            var fileToDownload = (SftpFile)menuItem.DataContext;
            await this.ViewModel.DownloadAsync(fileToDownload);
        }

        public async void OnAddFolder(object sender, RoutedEventArgs e)
        {
            await this.ViewModel.CreateFolderAsync();
            this.addFolderButton.Flyout.Hide();
        }

        public async void OnAddFile(object sender, RoutedEventArgs e)
        {
            await this.ViewModel.UploadAsync();
        }

        public async void OnFileMenuDeleteClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuFlyoutItem)sender;
            var fileToDelete = (SftpFile)menuItem.DataContext;
            await this.ViewModel.DeleteAsync(fileToDelete);
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
