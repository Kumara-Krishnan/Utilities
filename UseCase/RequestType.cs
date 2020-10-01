using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.UseCase
{
    public enum RequestType
    {
        LocalStorage,
        Network,
        LocalAndNetwork,
        Sync
    }

    public static class RequestTypeExtension
    {
        public static bool HasLocalStorage(this IUseCaseRequest request)
        {
            return request.Type == RequestType.LocalAndNetwork || request.Type == RequestType.LocalStorage
                || request.Type == RequestType.Sync;
        }

        public static bool HasNetwork(this IUseCaseRequest request)
        {
            return request.Type == RequestType.LocalAndNetwork || request.Type == RequestType.Network
                || request.Type == RequestType.Sync;
        }
    }
}
