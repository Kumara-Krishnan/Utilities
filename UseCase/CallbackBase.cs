using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.UseCase
{
    public interface ICallback<TResponse>
    {
        void OnSuccess(IUseCaseResponse<TResponse> response);

        void OnError(UseCaseError error);

        void OnFailed(IUseCaseResponse<TResponse> response);

        void OnCanceled(IUseCaseResponse<TResponse> response);
    }

    public interface ICallbackWithProgress<TResponse> : ICallback<TResponse>
    {
        void OnProgress(IUseCaseResponse<TResponse> response);
    }

    public abstract class CallbackBase<TResponse> : ICallbackWithProgress<TResponse>
    {
        public abstract void OnSuccess(IUseCaseResponse<TResponse> response);

        public abstract void OnError(UseCaseError error);

        public abstract void OnFailed(IUseCaseResponse<TResponse> response);

        public virtual void OnCanceled(IUseCaseResponse<TResponse> response) { }

        public virtual void OnProgress(IUseCaseResponse<TResponse> response) { }
    }
}
