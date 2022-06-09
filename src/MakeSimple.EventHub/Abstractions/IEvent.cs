namespace MakeSimple.EventHub.Abstractions
{
    public interface IEvent
    {
        public string EventId { get; set; }
    }
}