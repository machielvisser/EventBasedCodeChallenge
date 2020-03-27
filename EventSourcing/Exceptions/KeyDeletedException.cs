using System;

namespace EventSourcing.Exceptions
{
    public class KeyDeletedException : Exception
    {
        public KeyDeletedException(string message) : base(message)
        {

        }
    }
}
