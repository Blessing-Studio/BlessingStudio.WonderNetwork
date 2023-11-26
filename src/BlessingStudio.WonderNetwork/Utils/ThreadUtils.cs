using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BlessingStudio.WonderNetwork.Utils
{
    public static class ThreadUtils
    {
        public static void Run(Action<CancellationToken> action, TimeSpan timeout)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Thread thread = new(() =>
            {
                action(cancellationTokenSource.Token);
            });
            thread.Start();
            thread.Join(timeout);
            cancellationTokenSource.Cancel();
        }
    }
}
