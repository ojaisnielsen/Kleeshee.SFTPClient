using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kleeshee.SftpClient.DataModels.Mocks
{
    public class SftpFolderMock : ISftpFolder
    {
        private readonly List<ISftpFile> files = new List<ISftpFile>();

        private readonly List<ISftpFolder> folders = new List<ISftpFolder>();

        public async Task<ISftpFile> CreateFileAsync(string desiredName, CreationCollisionOption option)
        {
            return await this.CreateFileAsync(desiredName);
        }

        public async Task<ISftpFile> CreateFileAsync(string desiredName)
        {
            var newFile = new SftpFileMock { Name = desiredName, Path = Common.CombinePaths(this.Path, desiredName) };
            this.files.Add(newFile);
            return await Task.FromResult(newFile);
        }

        public async Task<ISftpFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option)
        {
            return await this.CreateFolderAsync(desiredName);
        }

        public async Task<ISftpFolder> CreateFolderAsync(string desiredName)
        {
            var newFolder = new SftpFolderMock { Name = desiredName, Path = Common.CombinePaths(this.Path, desiredName) };
            this.folders.Add(newFolder);
            return await Task.FromResult(newFolder);
        }

        public async Task<ISftpFile> GetFileAsync(string name)
        {
            var match = this.files.FirstOrDefault(file => file.Name == name);
            return await Task.FromResult(match);
        }

        public async Task<ISftpFolder> GetFolderAsync(string name)
        {
            var match = this.folders.FirstOrDefault(folder => folder.Name == name);
            return await Task.FromResult(match);
        }

        public async Task<ISftpItem> GetItemAsync(string name)
        {
            ISftpItem folderMatch = this.folders.FirstOrDefault(folder => folder.Name == name);
            ISftpItem fileMatch = this.files.FirstOrDefault(file => file.Name == name);
            return await Task.FromResult(fileMatch ?? folderMatch);
        }

        public async Task<IEnumerable<ISftpItem>> GetItemsAsync()
        {
            return await Task.FromResult(this.files.Concat<ISftpItem>(this.folders));
        }

        public IObservable<ISftpItem> GetPrefetchedItems()
        {
            return this.files.Concat<ISftpItem>(this.folders).ToObservable();
        }

        public string ImagePath { get { return "Assets/BlankFolder.png"; } }

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
