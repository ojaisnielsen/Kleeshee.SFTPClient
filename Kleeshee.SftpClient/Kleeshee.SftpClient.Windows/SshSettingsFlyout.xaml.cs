using Kleeshee.SftpClient.Common;
using Kleeshee.SftpClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace Kleeshee.SftpClient
{
    public sealed partial class SshSettingsFlyout : SettingsFlyout
    {
        public SshSettingsFlyout()
        {
            SshSettingsFlyout.instances.Add(this);
            this.viewModel = new SshSettingsViewModel(this.Dispatcher);
            this.InitializeComponent();
        }

        private static readonly HashSet<SshSettingsFlyout> instances = new HashSet<SshSettingsFlyout>();

        public static bool HasOpenInstance
        {
            get
            {
                return SshSettingsFlyout.instances.Any(instance => instance.IsOpen);
            }
        }

        public bool IsOpen
        {
            get
            {
                var parent = this.Parent as Popup;
                return parent != null && parent.IsOpen;
            }
        }

        private readonly ISshSettingsViewModel viewModel;

        public ISshSettingsViewModel ViewModel { get { return this.viewModel; } }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            this.ViewModel.SetPassword(passwordBox.Password);
        }

        private async void OnBrowseButtonClicked(object sender, RoutedEventArgs e)
        {
            await this.ViewModel.SetKey();
            this.Show();
        }

        private async void OnRemoveButtonClicked(object sender, RoutedEventArgs e)
        {
            await this.ViewModel.RemoveKey();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.ResetOnUnload)
            {
                ((App)Application.Current).DataSource.Reset();
            }
        }
    }
}
