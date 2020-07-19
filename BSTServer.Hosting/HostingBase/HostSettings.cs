using System.Text;

namespace BSTServer.Hosting.HostingBase
{
    public class HostSettings
    {
        public HostSettings()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public bool FixEnvironmentDirectory { get; set; } = false;
        public bool ShowWindow { get; set; } = false;
        public bool InputEnabled { get; set; } = false;

        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
}