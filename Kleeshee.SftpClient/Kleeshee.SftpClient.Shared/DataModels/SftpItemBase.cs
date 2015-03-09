using Kleeshee.SftpClient.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Core;

namespace Kleeshee.SftpClient.DataModels
{
    public abstract class SftpItemBase : ObservableBase, ISftpItem
    {
        private string imagePath;

        private double? progress = null;

        public static SftpItemBase Factory(Renci.SshNet.Sftp.SftpFile sftpFile, ISftpDataSource source, CoreDispatcher dispatcher)
        {
            if (sftpFile.IsDirectory)
            {
                return new SftpFolder(source, sftpFile, dispatcher) { ImagePath = "Assets/BlankFolder.png" };
            }
            else
            {
                return new SftpFile(source, sftpFile, dispatcher) { ImagePath = "Assets/Document.png" };
            }
        }

        public SftpItemBase(ISftpDataSource source, Renci.SshNet.Sftp.SftpFile file, CoreDispatcher dispatcher)
            : base(dispatcher)
        {
            this.SftpDataSource = source;
            this.SftpFile = file;
        }

        protected ISftpDataSource SftpDataSource { get; private set; }

        protected Renci.SshNet.Sftp.SftpFile SftpFile { get; private set; }

        public string ImagePath
        {
            get { return this.imagePath; }
            set
            {
                this.imagePath = value;
                this.OnPropertyChanged();
            }
        }

        public virtual FileAttributes Attributes
        {
            get
            {
                var attributes = FileAttributes.Normal;
                if (!this.SftpFile.GroupCanWrite && !this.SftpFile.OwnerCanWrite && !this.SftpFile.OthersCanWrite)
                {
                    attributes |= FileAttributes.ReadOnly;
                }

                return attributes;
            }
        }

        public double? Progress
        {
            get { return this.progress; }
            set
            {
                this.progress = value;
                this.OnPropertyChanged();
            }
        }

        public DateTimeOffset LastWriteDate
        {
            get { return this.SftpFile.LastWriteTimeUtc; }
        }

        public DateTimeOffset LastAccessDate
        {
            get { return this.SftpFile.LastAccessTimeUtc; }
        }

        public string Name
        {
            get { return this.SftpFile.Name; }
        }

        public string Path
        {
            get { return this.SftpFile.FullName; }
        }

        public async Task DeleteAsync()
        {
            await Task.Run(() => this.SftpFile.Delete());
        }

        public async Task RenameAsync(string desiredName)
        {
            await this.RenameAsync(desiredName, NameCollisionOption.FailIfExists);
        }

        public async Task RenameAsync(string desiredName, NameCollisionOption option)
        {
            var parentPath = this.Path.Substring(0, this.Path.Length - this.Name.Length);
            var newPath = await this.SftpDataSource.ResolveWriteablePathAsync(parentPath, desiredName, option);
            if (await this.SftpDataSource.ItemExistsAsync(newPath))
            {
                await this.SftpDataSource.DeleteItemAsync(newPath);
            }

            await Task.Run(() => this.SftpDataSource.SftpClient.RenameFile(this.SftpFile.FullName, newPath));
        }

        public virtual async Task PrefetchAsync()
        {
        }
    }
}
