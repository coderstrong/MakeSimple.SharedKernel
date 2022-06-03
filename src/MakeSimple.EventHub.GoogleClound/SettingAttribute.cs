using System;

namespace MakeSimple.EventHub.GoogleClound
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class SettingAttribute : Attribute
    {
        public string ProjectId { get; set; }
        public string TopicId { get; set; }
        public string SubscriptionId { get; set; }
    }
}
