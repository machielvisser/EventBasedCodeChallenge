using System;

namespace EventSourcing.Exceptions
{
    public class EmailInUseException : Exception
    {
        public EmailInUseException(string message) : base(message)
        {

        }
    }
}
