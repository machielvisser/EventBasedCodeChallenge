using EventSourcing.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcing.Interfaces
{
    /// <summary>
    /// Interface to an eventstore. It is assumed that the eventstore will deal with any conversions that are required for the storage medium and supports polymorphism.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Get all events of the given key.
        /// Note: Important improvements would be to add snapshots and a pub/sub mechanism to prevent reiterating the same events every time.
        /// </summary>
        /// <param name="key">A key that identifies a selection of events</param>
        /// <returns>IEnumerable of events for the key</returns>
        /// 
        Task<IEnumerable<EncryptedEvent>> Get(string key);

        /// <summary>
        /// Add one or more events to the event store for the provided key
        /// </summary>
        /// <param name="key">A key for the selection of events the provided events should be added to</param>
        /// <param name="events">The events to be added to the event store</param>
        Task Add(string key, IEnumerable<EncryptedEvent> events);
        Task Add(string key, EncryptedEvent @event);
    }
}
