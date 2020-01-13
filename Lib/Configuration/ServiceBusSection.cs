namespace Lib.Configuration
{
    public class ServiceBusSection
    {
        public string ServiceName { get; set; }
        public string Namespace { get; set; }
        public string SharedAccessKeyName { get; set; }
        public string SharedAccessKey { get; set; }
    }
}
