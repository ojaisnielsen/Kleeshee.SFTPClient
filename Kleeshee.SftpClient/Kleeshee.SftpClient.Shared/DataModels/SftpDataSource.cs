using Kleeshee.SftpClient.Common;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Core;

namespace Kleeshee.SftpClient.DataModels
{
    public class SftpDataSource : ISftpDataSource, IDisposable
    {
        public ConnectionInfo ConnectionInfo
        {
            get
            {
                object rawHost;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("host", out rawHost))
                {
                    rawHost = null;
                }

                var host = rawHost as string;
                if (string.IsNullOrEmpty(host))
                {
                    return null;
                }

                object rawUser;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("user", out rawUser))
                {
                    rawUser = null;
                }

                var user = rawUser as string;
                if (string.IsNullOrEmpty(user))
                {
                    return null;
                }

                object rawPort;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("port", out rawPort))
                {
                    rawPort = null;
                }

                var port = (rawPort as uint?) ?? 22;

                var authenticationMethods = new List<AuthenticationMethod>();

                var vault = new PasswordVault();
                PasswordCredential keyCredential;
                if (vault.TryGetCredential("key", null, out keyCredential))
                {
                    keyCredential.RetrievePassword();
                    using (var keyStream = keyCredential.Password.AsUtf8Stream())
                    {
                        authenticationMethods.Add(new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile(keyStream)));
                    }
                }

                PasswordCredential passwordCredential;
                if (vault.TryGetCredential("password", null, out passwordCredential))
                {
                    passwordCredential.RetrievePassword();
                    authenticationMethods.Add(new PasswordAuthenticationMethod(user, passwordCredential.Password));
                }

                if (authenticationMethods.Count == 0)
                {
                    return null;
                }

                return new ConnectionInfo(host, user, authenticationMethods.ToArray());
            }
        }

        public bool IsConnected
        {
            get { return this.SshClient != null && this.SftpClient != null; }
        }

        public Renci.SshNet.SftpClient SftpClient { get; private set; }
        public SshClient SshClient { get; private set; }

        public async Task ConnectAsync()
        {
            this.Dispose();
            var connectionInfo = this.ConnectionInfo;
            this.SshClient = new SshClient(connectionInfo);
            await this.SshClient.ConnectAsync();
            this.SftpClient = new Renci.SshNet.SftpClient(connectionInfo);
            await this.SftpClient.ConnectAsync();
        }

        public async Task<ISftpFolder> GetFolderAsync(string path, CoreDispatcher dispatcher)
        {
            var file = await Task.Run(() => this.SftpClient.Get(path));
            return new SftpFolder(this, file, dispatcher);
        }

        public async Task DeleteItemAsync(string path)
        {
            await Task.Run(() => this.SftpClient.Delete(path));
        }

        public async Task<bool> ItemExistsAsync(string path)
        {
            return await Task.Run(() => this.SftpClient.Exists(path));
        }

        public async Task<string> ResolveWriteablePathAsync(string directory, string desiredName, NameCollisionOption option)
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(desiredName);
            var extension = System.IO.Path.GetExtension(desiredName);
            var suffix = string.Empty;
            string newPath;
            for (var i = 1; await this.ItemExistsAsync(newPath = Common.CombinePaths(directory, fileName + suffix + extension)); suffix = string.Format(" ({0})", ++i))
            {
                if (option == NameCollisionOption.GenerateUniqueName)
                {
                    continue;
                }
                else if (option == NameCollisionOption.FailIfExists)
                {
                    throw new Exception("A file or folder with the desired name already exists.");
                }
                else
                {
                    break;
                }
            }

            return newPath;
        }

        public void Dispose()
        {
            if (this.SshClient != null)
            {
                this.SshClient.Dispose();
                this.SshClient = null;
            }

            if (this.SftpClient != null)
            {
                this.SftpClient.Dispose();
                this.SftpClient = null;
            }
        }

        public void Reset()
        {
            this.Dispose();
            this.OnReset();
        }

        public event EventHandler Reseted;

        protected async void OnReset()
        {
            if (this.Reseted != null)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Reseted(this, null));
            }
        }
    }
}
