using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.View.Contract;
using Windows.Foundation;
using Windows.UI.Core;

namespace Utilities.Extension
{
    public static class IViewExtension
    {
        public static IAsyncAction RunOnUIThread(this IView view, Action action,
            CoreDispatcherPriority priority = CoreDispatcherPriority.Normal, bool reThrow = false)
        {
            if (view is null) { return Task.CompletedTask.AsAsyncAction(); }

            return view.Dispatcher.RunOnUIThread(action, priority, reThrow);
        }
    }
}
