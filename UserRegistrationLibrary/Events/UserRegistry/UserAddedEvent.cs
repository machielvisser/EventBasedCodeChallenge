using EventSourcing.Entities;
using System;

namespace UserRegistrationLibrary.Events.UserRegistry
{
    public class UserAddedEvent : Event
    {
        public string Email { get; set; }
        public Guid UserId { get; set; }

        public UserAddedEvent(string email, Guid userId)
        {
            Email = email;
            UserId = userId;
        }
    }
}
