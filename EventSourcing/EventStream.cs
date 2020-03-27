using EventSourcing.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcing
{
    /// <summary>
    /// Base class for all aggregates that are stored in the event store
    /// </summary>
    public abstract class EventStream
    {
        private long _location;
        private readonly List<Event> _updates;

        public EventStream()
        {
            _location = 0;
            _updates = new List<Event>();
        }

        /// <summary>
        /// Gets the key of the aggregate to use in the stream id
        /// </summary>
        /// <returns></returns>
        public abstract Guid GetKey();

        /// <summary>
        /// Rehydrate the aggregate from the events
        /// </summary>
        /// <param name="events">The events</param>
        public void Rehydrate(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                Rehydrate(@event);
                _location = @event.Location;
            }
        }

        /// <summary>
        /// Abstract function to rehydrate from one event
        /// </summary>
        /// <param name="event"></param>
        public abstract void Rehydrate(Event @event);
        
        /// <summary>
        /// Update the aggregate with an event
        /// </summary>
        /// <param name="event">The event to add</param>
        protected void Update(Event @event)
        {
            @event.Location = _location + 1;
            Rehydrate(@event);
            _updates.Add(@event);
        }

        /// <summary>
        /// Commit the change to the event store
        /// </summary>
        /// <param name="commit">Implementation of the commit</param>
        public async Task Commit(Func<IEnumerable<Event>, Task> commit)
        {
            await commit(_updates);

            _updates.Clear();
        }
    }
}
