using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

using Kleeshee.SftpClient.Common;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace Kleeshee.SftpClient.DataModels
{
    public class SftpFile : SftpItemBase, ISftpFile
    {
        private readonly Task contentTypeTask;

        private string contentType = null;

        private ulong size;

        public SftpFile(ISftpDataSource source, Renci.SshNet.Sftp.SftpFile sftpFile, CoreDispatcher dispatcher)
            : base(source, sftpFile, dispatcher)
        {
            if (sftpFile.IsDirectory)
            {
                throw new ArgumentException("The SFTP item is a directory.");
            }

            this.Size = (ulong)this.SftpFile.Length;

            var contentTypeCommand = this.SftpDataSource.SshClient.CreateCommand(string.Format("file -b --mime-type \"{0}\"", this.Path));
            this.contentTypeTask = contentTypeCommand.ExecuteAsync().ContinueWith(async contentType => 
            { 
                this.ContentType = (await contentType).TrimEnd('\r', '\n');
            });
        }

        public string ContentType 
        {
            get { return this.contentType; }
            set 
            {
                this.contentType = value;
                this.OnPropertyChanged();
            }
        }

        public ulong Size
        {
            get { return this.size; }
            set
            {
                this.size = value;
                this.OnPropertyChanged();
            }
        }

        public string FileType
        {
            get { return System.IO.Path.GetExtension(this.Name); }
        }

        public override async Task PrefetchAsync()
        {
            await this.contentTypeTask;
        }

        public async Task CopyAndReplaceAsync(IStorageFile fileToReplace)
        {
            using (var targetStream = await fileToReplace.OpenStreamForWriteAsync())
            {
                await this.SftpDataSource.SftpClient.DownloadFileAsync(this.Path, targetStream);
            }
        }

        public async Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            var destinationFile = await destinationFolder.CreateFileAsync(desiredNewName, (CreationCollisionOption)option);
            await this.CopyAndReplaceAsync(destinationFile);
            return destinationFile;
        }

        public AsyncActionWithProgress<ulong> DownloadAsyncWithProgress(Stream targetStream)
        {
            return this.SftpDataSource.SftpClient.DownloadAsyncWithProgress(this.Path, targetStream);
        }

        public AsyncActionWithProgress<ulong> CopyAndReplaceAsyncWithProgress(Stream input)
        {
            return this.SftpDataSource.SftpClient.UploadAsyncWithProgress(input, this.Path);
        }

        public async Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            return await this.CopyAsync(destinationFolder, desiredNewName, NameCollisionOption.FailIfExists);
        }

        public async Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder)
        {
            return await this.CopyAsync(destinationFolder, this.Name, NameCollisionOption.FailIfExists);
        }

        public async Task MoveAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            await this.MoveAsync(destinationFolder, desiredNewName, NameCollisionOption.FailIfExists);
        }

        public async Task MoveAsync(IStorageFolder destinationFolder)
        {
            await this.MoveAsync(destinationFolder, this.Name, NameCollisionOption.FailIfExists);
        }

        public async Task MoveAndReplaceAsync(IStorageFile fileToReplace)
        {
            await this.CopyAndReplaceAsync(fileToReplace);
            await this.DeleteAsync();
        }

        public async Task MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            await this.CopyAsync(destinationFolder, desiredNewName, option);
            await this.DeleteAsync();
        }

        public async Task<IRandomAccessStream> OpenAsync(FileAccessMode accessMode)
        {
            var fileAccess = accessMode == FileAccessMode.ReadWrite ? FileAccess.ReadWrite : FileAccess.Read;
            var stream = await Task.Run<Stream>(() => this.SftpDataSource.SftpClient.Open(this.Path, FileMode.Open, fileAccess));
            return stream.AsRandomAccessStream();
        }

        public async Task<IRandomAccessStreamWithContentType> OpenReadAsync()
        {
            var stream = await this.OpenAsync(FileAccessMode.Read);
            return await Task.Run<IRandomAccessStreamWithContentType>(() => stream.AsRandomAccessStreamWithContentType(this.ContentType));
        }
    }
}
