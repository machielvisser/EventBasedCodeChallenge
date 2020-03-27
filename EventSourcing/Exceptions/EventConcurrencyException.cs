using System;

namespace EventSourcing.Exceptions
{
    public class EventConcurrencyException : Exception
    {
        public EventConcurrencyException(string message) : base(message) { }
    }
}
