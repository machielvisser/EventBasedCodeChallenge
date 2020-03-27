using EventSourcing.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcing
{
    public class CommandProcessor
    {
        private readonly EventRepository _eventRepository;

        public CommandProcessor(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        /// <summary>
        /// Process a command on an existing stream
        /// Throws an exception in case the event store cannot be reached or the stream has been updated by antoher client in the meantime
        /// </summary>
        /// <typeparam name="TEntity">The entity type the command will be applied to</typeparam>
        /// <param name="aggregateId">The key that indicates the aggregate</param>
        /// <param name="command">The command that needs to be applied</param>
        /// <returns></returns>
        public async Task Process<TEntity>(Guid aggregateId, Action<TEntity> command) where TEntity : EventStream, new()
        {
            var events = await _eventRepository.Get<TEntity>(aggregateId);

            var entity = new TEntity();
            entity.Rehydrate(events);

            command(entity);

            await Commit(entity);
        }

        /// <summary>
        /// Commit the changes to the event store
        /// Can throw an exception in case the event store cannot be reached or when the stream has been updated by another client in the meantime.
        /// </summary>
        /// <param name="entity">The event stream to be committed</param>
        /// <returns></returns>
        private async Task Commit<T>(T entity) where T : EventStream
        {
            await entity.Commit(async (IEnumerable<Event> events) => await _eventRepository.Add<T>(entity.GetKey(), events));
        }
    }
}
