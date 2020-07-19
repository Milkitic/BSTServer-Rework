using System;

namespace BSTServer.Hosting.HostingBase
{
    public class OutputReceivedEventArgs : EventArgs
    {
        internal OutputReceivedEventArgs(string data, bool isErrorOutput)
        {
            Data = data;
            IsErrorOutput = isErrorOutput;
        }

        public string Data { get; }
        public bool IsErrorOutput { get; }
    }
}