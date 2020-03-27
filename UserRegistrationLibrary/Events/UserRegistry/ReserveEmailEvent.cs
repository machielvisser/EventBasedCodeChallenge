using EventSourcing.Entities;
using System;

namespace UserRegistrationLibrary.Events.UserRegistry
{
    public class ReserveEmailEvent : Event
    {
        public string Email { get; set; }
        public Guid UserId { get; set; }

        public ReserveEmailEvent(string email, Guid userId)
        {
            Email = email;
            UserId = userId;
        }
    }
}
