namespace Milde.EventStore.Entities
{
    class EventMetadata
    {
        public EventMetadata(string assembly, string eventType)
        {
            Assembly = assembly;
            EventType = eventType;
        }

        public string Assembly { get; private set; }
        public string EventType { get; private set; }
    }
}
