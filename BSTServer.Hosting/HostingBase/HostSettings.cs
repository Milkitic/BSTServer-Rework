using System.Text;

namespace BSTServer.Hosting.HostingBase
{
    public class HostSettings
    {
        public HostSettings()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public bool FixEnvironmentDirectory { get; set; } = true;
        public bool InputEnabled { get; set; } = true;
        public bool ShowWindow { get; set; } = false;

        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
}