using Kleeshee.SftpClient.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Kleeshee.SftpClient.DataModels.Mocks
{
    public class SftpFileMock : ISftpFile 
    {
        public string ContentType { get; set;}

        public ulong Size { get; set; }

        public string FileType
        {
            get { return System.IO.Path.GetExtension(this.Name); }
        }

        public async Task CopyAndReplaceAsync(IStorageFile fileToReplace)
        {
        }

        public async Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            return await Task.FromResult(default(IStorageFile));
        }

        public AsyncActionWithProgress<ulong> DownloadAsyncWithProgress(Stream targetStream)
        {
            return null;
        }

        public AsyncActionWithProgress<ulong> CopyAndReplaceAsyncWithProgress(Stream input)
        {
            return null;
        }

        public async Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            return await Task.FromResult(default(IStorageFile));
        }

        public async Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder)
        {
            return await Task.FromResult(default(IStorageFile));
        }

        public async Task MoveAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
        }

        public async Task MoveAsync(IStorageFolder destinationFolder)
        {
        }

        public async Task MoveAndReplaceAsync(IStorageFile fileToReplace)
        {
        }

        public async Task MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
        }

        public async Task<IRandomAccessStream> OpenAsync(Windows.Storage.FileAccessMode accessMode)
        {
            return await Task.FromResult(default(IRandomAccessStream));
        }

        public async Task<IRandomAccessStreamWithContentType> OpenReadAsync()
        {
            return await Task.FromResult(default(IRandomAccessStreamWithContentType));
        }

        public string ImagePath { get { return "Assets/Document.png"; } }

        public FileAttributes Attributes { get; set; }

        public double? Progress { get; set; }

        public DateTimeOffset LastWriteDate { get; set; }

        public DateTimeOffset LastAccessDate { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public async Task DeleteAsync()
        {
        }

        public async Task RenameAsync(string desiredName)
        {
        }

        public async Task RenameAsync(string desiredName, NameCollisionOption option)
        {
        }

        public async Task PrefetchAsync()
        {
        }
    }
}
