namespace MakeSimple.EventHub.GoogleClound
{
    public class EventHubSettingOption
    {
        public bool IsSubscription { get; set; } = false;

        public string Suffix { get; set; }

        public string ProjectId { get; set; }
        public bool IsSimulatorMode { get; set; } = false;
    }
}