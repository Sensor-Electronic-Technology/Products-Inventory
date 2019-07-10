namespace Inventory.Common.BuisnessLayer {
    using Prism.Logging.Syslog;

    public interface ISyslogConfig {
        string AppNameOrTag { get; set; }
        string HostNameOrIp { get; set; }
        int? Port { get; set; }
    }
}
