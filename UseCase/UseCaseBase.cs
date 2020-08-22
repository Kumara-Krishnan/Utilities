using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Error;
using Utilities.Extension;

namespace Utilities.UseCase
{
    public abstract class UseCaseBase<TRequest, TResponse> where TRequest : IUseCaseRequest
    {
        public TRequest Request { get; private set; }

        protected ICallback<TResponse> PresenterCallback { get; private set; }

        private CancellationToken CancellationToken { get { return Request?.CTS?.Token ?? default; } }

        public UseCaseBase(TRequest request, ICallback<TResponse> presenterCallback = null)
        {
            Request = request;
            PresenterCallback = presenterCallback;
        }

        public async void Execute()
        {
            try
            {
                GetFromCache();
            }
            catch { }

            try
            {
                await Task.Run(async () =>
                {
                    await Action().ConfigureAwait(false);
                }, CancellationToken).ConfigureAwait(false);
            }
            catch (NoInternetAccessException nie)
            {
                PresenterCallback?.OnError(ErrorType.Network, nie);
            }
            catch (OperationCanceledException)
            {
                PresenterCallback?.OnCanceled(ResponseType.Unknown);
            }
            catch (Exception exception)
            {
                PresenterCallback?.OnError(ErrorType.Unknown, exception);
            }
        }

        protected virtual bool GetFromCache()
        {
            return false;
        }

        protected abstract Task Action();
    }
}
