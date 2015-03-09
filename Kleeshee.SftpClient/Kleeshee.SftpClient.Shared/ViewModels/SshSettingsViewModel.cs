using Kleeshee.SftpClient.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;

namespace Kleeshee.SftpClient.ViewModels
{
    public class SshSettingsViewModel : ObservableBase, ISshSettingsViewModel
    {
        public SshSettingsViewModel(CoreDispatcher dispatcher) : base(dispatcher)
        {
        }

        public string Host
        {
            get
            {
                object host;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("host", out host))
                {
                    host = null;
                }

                return (host as string) ?? string.Empty;
            }

            set
            {
                if (value != this.Host)
                {
                    ApplicationData.Current.RoamingSettings.Values["host"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string User
        {
            get
            {
                object user;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("user", out user))
                {
                    user = null;
                }

                return (user as string) ?? string.Empty;
            }

            set
            {
                if (value != this.User)
                {
                    ApplicationData.Current.RoamingSettings.Values["user"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public uint Port
        {
            get
            {
                object port;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("port", out port))
                {
                    port = null;
                }

                return (port as uint?) ?? 22u;
            }

            set
            {
                if (value != this.Port)
                {
                    ApplicationData.Current.RoamingSettings.Values["port"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string KeyFilePath
        {
            get
            {
                object path;
                if (!ApplicationData.Current.LocalSettings.Values.TryGetValue("keyFilePath", out path))
                {
                    path = null;
                }

                return (path as String) ?? string.Empty;
            }

            set
            {
                if (value != this.KeyFilePath)
                {
                    ApplicationData.Current.LocalSettings.Values["keyFilePath"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void SetPassword(string password)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential("password", null, password));
        }

        public async Task SetKey()
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add("*");
            filePicker.ViewMode = PickerViewMode.List;
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            var keyFile = await filePicker.PickSingleFileAsync();
            var vault = new PasswordVault();
            if (keyFile == null)
            {
                vault.RemoveAll("key", null);
            }
            else
            {
                this.KeyFilePath = keyFile.Path;
                vault.Add(new PasswordCredential("key", null, await FileIO.ReadTextAsync(keyFile)));
            }
        }
    }
}
