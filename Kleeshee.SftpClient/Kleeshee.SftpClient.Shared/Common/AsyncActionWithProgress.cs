using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Kleeshee.SftpClient.Common
{
    public class AsyncActionWithProgress<TProgress> : IAsyncAction, IAsyncActionWithProgress<TProgress>
    {
        private readonly IAsyncAction asyncAction;

        public AsyncActionWithProgress(Func<AsyncCallback, object, Action<TProgress>, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object state)
        {
            this.asyncAction = Task.Factory.FromAsync((callback, s) => beginMethod(callback, s, progress =>
            {
                if (this.Progress != null) this.Progress(this, progress);
            }), asyncResult => endMethod(asyncResult), state).AsAsyncAction();

            this.asyncAction.Completed = (asyncInfo, asyncStatus) =>
            {
                if (((IAsyncAction)this).Completed != null)
                {
                    ((IAsyncAction)this).Completed(this, asyncStatus);
                }

                if (((IAsyncActionWithProgress<TProgress>)this).Completed != null)
                {
                    ((IAsyncActionWithProgress<TProgress>)this).Completed(this, asyncStatus);
                }
            };
        }

        public AsyncActionCompletedHandler Completed { get; set; }

        public void GetResults()
        {
            this.asyncAction.GetResults();
        }

        public void Cancel()
        {
            this.asyncAction.Cancel();
        }

        public void Close()
        {
            this.asyncAction.Close();
        }

        public Exception ErrorCode
        {
            get { return this.asyncAction.ErrorCode; }
        }

        public uint Id
        {
            get { return this.asyncAction.Id; }
        }

        public AsyncStatus Status
        {
            get { return this.asyncAction.Status; }
        }

        AsyncActionWithProgressCompletedHandler<TProgress> IAsyncActionWithProgress<TProgress>.Completed { get; set; }

        public AsyncActionProgressHandler<TProgress> Progress { get; set; }
    }
}
