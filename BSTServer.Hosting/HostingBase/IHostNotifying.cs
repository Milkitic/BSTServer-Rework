namespace BSTServer.Hosting.HostingBase
{
    public interface IHostNotifying
    {
        event OutputReceivedEventHandler DataReceived;
    }
}