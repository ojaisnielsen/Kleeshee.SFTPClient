using Kleeshee.SftpClient.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kleeshee.SftpClient.DataModels
{
    public static class Common
    {
        public static AsyncActionWithProgress<ulong> DownloadAsyncWithProgress(this Renci.SshNet.SftpClient sftpClient, string path, Stream targetStream)
        {
            return new AsyncActionWithProgress<ulong>((callBack, state, progressCallback) => sftpClient.BeginDownloadFile(path, targetStream, callBack, state, progressCallback), asyncResult => sftpClient.EndDownloadFile(asyncResult), null);
        }

        public static AsyncActionWithProgress<ulong> UploadAsyncWithProgress(this Renci.SshNet.SftpClient sftpClient, Stream input, string targetPath)
        {
            return new AsyncActionWithProgress<ulong>((callBack, state, progressCallback) => sftpClient.BeginUploadFile(input, targetPath, callBack, state, progressCallback), asyncResult => sftpClient.EndUploadFile(asyncResult), null);
        }

        public static string CombinePaths(params string[] fragments)
        {
            return string.Join("/", from fragment in fragments select fragment.Replace('\\', '/').TrimEnd('/'));
        }
    }
}
