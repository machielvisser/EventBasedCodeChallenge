using EventSourcing.Entities;
using System;

namespace UserRegistrationLibrary.Events.UserRegistry
{
    public class EmailVerifiedEvent : Event
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }

        public EmailVerifiedEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
