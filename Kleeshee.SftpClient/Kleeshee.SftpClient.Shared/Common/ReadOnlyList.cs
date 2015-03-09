using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Kleeshee.SftpClient.Common
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> list;

        public ReadOnlyList(IList<T> list)
        {
            this.list = list;
        }

        public T this[int index]
        {
            get { return this.list[index]; }
        }

        public int Count
        {
            get { return this.list.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
    }
}
