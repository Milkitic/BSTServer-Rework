using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BSTServer.Hosting.HostingBase
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Waits asynchronously for the process to exit.
        /// </summary>
        /// <param name="process">The process to wait for cancellation.</param>
        /// <param name="cancellationToken">A cancellation token. If invoked, the task will return 
        /// immediately as canceled.</param>
        /// <returns>A Task representing waiting for the process to end.</returns>
        public static async Task<bool> WaitForExitAsync(this Process process,
            CancellationToken cancellationToken = default)
        {
            if (process.HasExited) return true;

            var tcs = new TaskCompletionSource<bool>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(true);
            if (cancellationToken != default)
                cancellationToken.Register(() => tcs.TrySetResult(false)/*tcs.SetCanceled()*/);

            return process.HasExited || await tcs.Task;
        }

        public static async Task<bool> CloseMainWindowAsync(this Process process,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(process.CloseMainWindow, cancellationToken);
        }

        public static async Task KillAsync(this Process process,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(process.Kill, cancellationToken);
        }
    }
}
