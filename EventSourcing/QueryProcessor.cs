using System;
using System.Threading.Tasks;

namespace EventSourcing
{
    /// <summary>
    /// Processes a query on the data in the event store
    /// </summary>
    public class QueryProcessor
    {
        private readonly EventRepository _eventRepository;

        public QueryProcessor(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        /// <summary>
        /// Retrieves an aggregate and runs the provided query on the aggregate
        /// </summary>
        /// <typeparam name="T">Aggregate type</typeparam>
        /// <typeparam name="R">Type of the query output</typeparam>
        /// <param name="id">Id of the aggregate</param>
        /// <param name="query">The function that performs the query</param>
        public async Task<R> Query<T,R>(Guid id, Func<T, R> query) where T : EventStream, new()
        {
            var events = await _eventRepository.Get<T>(id);

            var entity = new T();
            entity.Rehydrate(events);

            return query(entity);
        }
    }
}
