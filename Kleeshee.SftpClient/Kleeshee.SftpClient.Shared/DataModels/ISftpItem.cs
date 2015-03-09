using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kleeshee.SftpClient.DataModels
{
    public interface ISftpItem
    {
        string ImagePath { get; }

        FileAttributes Attributes { get; }

        double? Progress { get; set; }

        DateTimeOffset LastWriteDate { get; }

        DateTimeOffset LastAccessDate { get; }

        string Name { get; }

        string Path { get; }

        Task DeleteAsync();

        Task RenameAsync(string desiredName);

        Task RenameAsync(string desiredName, NameCollisionOption option);

        Task PrefetchAsync();
    }
}
