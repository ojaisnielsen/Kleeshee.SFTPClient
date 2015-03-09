using Kleeshee.SftpClient.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Kleeshee.SftpClient.DataModels
{
    public interface ISftpFile : ISftpItem
    {
        string ContentType { get; set; }

        ulong Size { get; set; }

        string FileType { get; }

        Task CopyAndReplaceAsync(IStorageFile fileToReplace);

        Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);

        AsyncActionWithProgress<ulong> DownloadAsyncWithProgress(Stream targetStream);

        AsyncActionWithProgress<ulong> CopyAndReplaceAsyncWithProgress(Stream input);

        Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName);

        Task<IStorageFile> CopyAsync(IStorageFolder destinationFolder);

        Task MoveAsync(IStorageFolder destinationFolder, string desiredNewName);

        Task MoveAsync(IStorageFolder destinationFolder);

        Task MoveAndReplaceAsync(IStorageFile fileToReplace);

        Task MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);

        Task<IRandomAccessStream> OpenAsync(FileAccessMode accessMode);

        Task<IRandomAccessStreamWithContentType> OpenReadAsync();
    }
}
