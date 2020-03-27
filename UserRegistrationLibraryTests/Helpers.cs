using EventSourcing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserRegistrationLibraryTests
{
    public static class Helpers
    {

        public static bool Exists<TEntity>(this IEnumerable<Event> stream, Func<TEntity, bool> func) where TEntity : Event
        {
            return stream
                .Where(@event => @event is TEntity)
                .Select(@event => @event as TEntity)
                .Any(func);
        }
    }
}
