using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;

namespace Kleeshee.SftpClient.DataModels
{
    public interface ISftpDataSource
    {
        bool IsConnected { get; }

        Renci.SshNet.SftpClient SftpClient { get; }

        Renci.SshNet.SshClient SshClient { get; }

        Task ConnectAsync();

        Task<ISftpFolder> GetFolderAsync(string path, CoreDispatcher dispatcher);

        Task DeleteItemAsync(string path);

        Task<bool> ItemExistsAsync(string path);

        Task<string> ResolveWriteablePathAsync(string directory, string desiredName, NameCollisionOption option);

        void Reset();

        event EventHandler Reseted;
    }
}
