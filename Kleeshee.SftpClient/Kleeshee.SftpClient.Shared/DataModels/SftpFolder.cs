using Kleeshee.SftpClient.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;

namespace Kleeshee.SftpClient.DataModels
{
    public class SftpFolder : SftpItemBase, ISftpFolder
    {
        public SftpFolder(ISftpDataSource source, Renci.SshNet.Sftp.SftpFile file, CoreDispatcher dispatcher)
            : base(source, file, dispatcher)
        {
            if (!file.IsDirectory)
            {
                throw new ArgumentException("The SFTP item is not a directory.");
            }
        }

        public override FileAttributes Attributes
        {
            get
            {
                return base.Attributes | FileAttributes.Directory;
            }
        }

        public async Task<ISftpFile> CreateFileAsync(string desiredName, CreationCollisionOption option)
        {
            var newPath = await this.SftpDataSource.ResolveWriteablePathAsync(this.Path, desiredName, (NameCollisionOption)option);
            var itemExists = await this.SftpDataSource.ItemExistsAsync(newPath);
            if (itemExists && option == CreationCollisionOption.OpenIfExists)
            {
                var file = await Task.Run(() => this.SftpDataSource.SftpClient.Get(newPath));
                if (!file.IsDirectory)
                {
                    return new SftpFile(this.SftpDataSource, file, this.Dispatcher);
                }
            }

            if (itemExists)
            {
                await this.SftpDataSource.DeleteItemAsync(newPath);
            }

            var newFile = await Task.Run(() =>
            {
                this.SftpDataSource.SftpClient.Create(newPath).Dispose();
                return this.SftpDataSource.SftpClient.Get(newPath);
            });

            return new SftpFile(this.SftpDataSource, newFile, this.Dispatcher);
        }

        public async Task<ISftpFile> CreateFileAsync(string desiredName)
        {
            return await this.CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);
        }

        public async Task<ISftpFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option)
        {
            var newPath = await this.SftpDataSource.ResolveWriteablePathAsync(this.Path, desiredName, (NameCollisionOption)option);
            var itemExists = await this.SftpDataSource.ItemExistsAsync(newPath);
            if (itemExists && option == CreationCollisionOption.OpenIfExists)
            {
                var file = await Task.Run(() => this.SftpDataSource.SftpClient.Get(newPath));
                if (file.IsDirectory)
                {
                    return new SftpFolder(this.SftpDataSource, file, this.Dispatcher);
                }
            }

            if (itemExists)
            {
                await this.SftpDataSource.DeleteItemAsync(newPath);
            }

            var newFile = await Task.Run(() =>
            {
                this.SftpDataSource.SftpClient.CreateDirectory(newPath);
                return this.SftpDataSource.SftpClient.Get(newPath);
            });

            return new SftpFolder(this.SftpDataSource, newFile, this.Dispatcher);
        }

        public async Task<ISftpFolder> CreateFolderAsync(string desiredName)
        {
            return await this.CreateFolderAsync(desiredName, CreationCollisionOption.FailIfExists);
        }

        public async Task<ISftpFile> GetFileAsync(string name)
        {
            var file = await Task.Run(() => this.SftpDataSource.SftpClient.Get(Common.CombinePaths(this.Path, name)));
            return new SftpFile(this.SftpDataSource, file, this.Dispatcher);
        }

        public async Task<ISftpFolder> GetFolderAsync(string name)
        {
            var item = await Task.Run(() => this.SftpDataSource.SftpClient.Get(Common.CombinePaths(this.Path, name)));
            return new SftpFolder(this.SftpDataSource, item, this.Dispatcher);
        }

        public async Task<ISftpItem> GetItemAsync(string name)
        {
            var item = await Task.Run(() => this.SftpDataSource.SftpClient.Get(Common.CombinePaths(this.Path, name)));
            return SftpItemBase.Factory(item, this.SftpDataSource, this.Dispatcher);
        }

        public async Task<IEnumerable<ISftpItem>> GetItemsAsync()
        {
            var items = await this.SftpDataSource.SftpClient.ListDirectoryAsync(this.Path);
            return from item in items 
                   orderby item.Name
                   select SftpItemBase.Factory(item, this.SftpDataSource, this.Dispatcher);
        }

        public IObservable<ISftpItem> GetPrefetchedItems()
        {
            return Observable.Create<ISftpItem>(async obs =>
            {
                foreach (var item in await this.GetItemsAsync())
                {
                    await item.PrefetchAsync();
                    obs.OnNext(item);
                }
            });
        }
    }
}
