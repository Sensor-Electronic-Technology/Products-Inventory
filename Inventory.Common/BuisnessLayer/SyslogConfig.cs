namespace Inventory.Common.BuisnessLayer {
    using Prism.Logging.Syslog;

    public class SyslogConfig : ISyslogConfig, ISyslogOptions {
        public string AppNameOrTag { get; set; }
        public string HostNameOrIp { get; set; }
        public int? Port { get; set; }
    }
}
