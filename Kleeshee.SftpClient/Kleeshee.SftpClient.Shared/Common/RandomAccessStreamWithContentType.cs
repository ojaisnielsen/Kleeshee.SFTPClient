using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Streams;

namespace Kleeshee.SftpClient.Common
{
    public class RandomAccessStreamWithContentType : IRandomAccessStreamWithContentType
    {
        private readonly IRandomAccessStream sftpRandomAccessStream;

        public RandomAccessStreamWithContentType(IRandomAccessStream randomAccessStream, string contentType)
        {
            this.sftpRandomAccessStream = randomAccessStream;
            this.ContentType = contentType;
        }

        public bool CanRead
        {
            get { return this.sftpRandomAccessStream.CanRead; }
        }

        public bool CanWrite
        {
            get { return this.sftpRandomAccessStream.CanWrite; }
        }

        public IRandomAccessStream CloneStream()
        {
            return this.sftpRandomAccessStream.CloneStream();
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            return this.sftpRandomAccessStream.GetInputStreamAt(position);
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            return this.sftpRandomAccessStream.GetOutputStreamAt(position);
        }

        public ulong Position
        {
            get { return this.sftpRandomAccessStream.Position; }
        }

        public void Seek(ulong position)
        {
            this.sftpRandomAccessStream.Seek(position);
        }

        public ulong Size
        {
            get
            {
                return this.sftpRandomAccessStream.Size;
            }
            set
            {
                this.sftpRandomAccessStream.Size = value;
            }
        }

        public void Dispose()
        {
            this.sftpRandomAccessStream.Dispose();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return this.sftpRandomAccessStream.ReadAsync(buffer, count, options);
        }

        public Windows.Foundation.IAsyncOperation<bool> FlushAsync()
        {
            return this.sftpRandomAccessStream.FlushAsync();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return this.sftpRandomAccessStream.WriteAsync(buffer);
        }

        public string ContentType
        {
            get;
            private set;
        }
    }
}
