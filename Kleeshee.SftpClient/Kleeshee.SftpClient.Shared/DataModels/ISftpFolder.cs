using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kleeshee.SftpClient.DataModels
{
    public interface ISftpFolder : ISftpItem
    {
        Task<ISftpFile> CreateFileAsync(string desiredName, CreationCollisionOption option);

        Task<ISftpFile> CreateFileAsync(string desiredName);

        Task<ISftpFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option);

        Task<ISftpFolder> CreateFolderAsync(string desiredName);

        Task<ISftpFile> GetFileAsync(string name);

        Task<ISftpFolder> GetFolderAsync(string name);

        Task<ISftpItem> GetItemAsync(string name);

        Task<IEnumerable<ISftpItem>> GetItemsAsync();

        IObservable<ISftpItem> GetPrefetchedItems();
    }
}
