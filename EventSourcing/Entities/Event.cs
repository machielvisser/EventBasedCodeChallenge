using System;

namespace EventSourcing.Entities
{
    /// <summary>
    /// Base class for an event
    /// </summary>
    public class Event
    {
        public Guid EventId { get; set; }
        public long Location { get; set; }

        public Event()
        {
            EventId = Guid.NewGuid();
        }
    }
}
