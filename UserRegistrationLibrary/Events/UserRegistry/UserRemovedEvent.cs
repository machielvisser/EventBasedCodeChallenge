using EventSourcing.Entities;
using System;

namespace UserRegistrationLibrary.Events.UserRegistry
{
    public class UserRemovedEvent : Event
    {
        public Guid UserId { get; set; }

        public UserRemovedEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
