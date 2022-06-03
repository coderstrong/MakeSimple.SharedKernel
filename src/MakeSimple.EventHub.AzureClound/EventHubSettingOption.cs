using System.Reflection;

namespace MakeSimple.EventHub.Abstractions
{
    public class EventHubSettingOption
    {
        public bool IsSubscription { get; set; }

        public string Suffix { get; set; }

        public string EventHubName { get; set; }

        public string ConnectionString { get; set; }
    }
}
