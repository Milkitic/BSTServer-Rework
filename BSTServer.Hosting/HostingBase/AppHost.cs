using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BSTServer.Hosting.HostingBase
{
    public abstract class AppHost : IHostNotifying
    {
        public event OutputReceivedEventHandler DataReceived;
        public event Action ProcessExited;

        private Process _processInstance;

        public Guid Guid { get; set; } = Guid.NewGuid();
        public string FileName { get; set; }
        public string Args { get; set; }

        public HostSettings HostSettings { get; set; }

        public Guid InstanceGuid { get; private set; }

        public bool IsRunning => _processInstance?.HasExited ?? false;
        public bool IsInputEnabled => HostSettings.InputEnabled;

        public AppHost()
        {
        }

        public async Task RunInBackground()
        {
            await StartProcessAsync();
        }

        public async Task RunAsync()
        {
            await RunInBackground();
            await _processInstance.WaitForExitAsync();
        }

        public async Task StopAsync()
        {
            await StopProcessAsync(20000);
        }

        public async Task KillAsync()
        {
            await _processInstance.KillAsync();
        }

        public void SendMessage(string message)
        {
            if (HostSettings.InputEnabled == false)
                return;
            _processInstance.StandardInput.Write(message + _processInstance.StandardInput.NewLine);
        }

        public override string ToString()
        {
            return $"{{{Guid}}} {base.ToString()}";
        }

        protected virtual Task OnStarted()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnDataReceived(object sender, OutputReceivedEventArgs e)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnExited()
        {
            return Task.CompletedTask;
        }

        private async Task StartProcessAsync()
        {
            await StopAsync();
            _processInstance = new Process
            {
                StartInfo =
                {
                    FileName = FileName,
                    Arguments = Args,
                    CreateNoWindow = !HostSettings.ShowWindow,
                    StandardOutputEncoding = HostSettings.Encoding,
                    UseShellExecute = false,
                    RedirectStandardInput = HostSettings.InputEnabled,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = HostSettings.ShowWindow ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                },
                EnableRaisingEvents = true
            };

            if (HostSettings.FixEnvironmentDirectory)
                _processInstance.StartInfo.WorkingDirectory = Path.GetDirectoryName(FileName);

            _processInstance.OutputDataReceived += ProcessInstance_OutputDataReceived;
            _processInstance.ErrorDataReceived += ProcessInstance_ErrorDataReceived;
            _processInstance.Exited += ProcessInstance_Exited;

            _processInstance.Start();
            _processInstance.BeginOutputReadLine();
            _processInstance.BeginErrorReadLine();
            InstanceGuid = Guid.NewGuid();
            OnStarted();

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Process \"{_processInstance.ProcessName}\" started.");
        }

        private void ProcessInstance_Exited(object sender, EventArgs e)
        {
            _processInstance.OutputDataReceived -= ProcessInstance_OutputDataReceived;
            _processInstance.ErrorDataReceived -= ProcessInstance_ErrorDataReceived;
            OnExited();
            ProcessExited?.Invoke();
        }

        private void ProcessInstance_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnDataReceived(sender, new OutputReceivedEventArgs(e.Data, true));
            DataReceived?.Invoke(sender, new OutputReceivedEventArgs(e.Data, true));
        }
        private void ProcessInstance_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataReceived?.Invoke(sender, new OutputReceivedEventArgs(e.Data, false));
        }

        private async Task StopProcessAsync(int killTimeout)
        {
            if (_processInstance == null) return;
            if (_processInstance.HasExited) return;

            if (await _processInstance.CloseMainWindowAsync())
            {
                if (!_processInstance.WaitForExit(killTimeout))
                {
                    await _processInstance.KillAsync();
                }
            }
            else if (await CloseGracefully())
            {
                if (!_processInstance.WaitForExit(killTimeout))
                {
                    await _processInstance.KillAsync();
                }
            }
            else
            {
                await _processInstance.KillAsync();
            }
        }

        protected abstract Task<bool> CloseGracefully();
    }
}
