using EventSourcing.Entities;
using System;

namespace UserRegistrationLibrary.Events.User
{
    internal class UserEvent : Event
    {
        public Guid UserId { get; set; }

        public UserEvent(Guid userId) : base()
        {
            UserId = userId;
        }
    }
}
