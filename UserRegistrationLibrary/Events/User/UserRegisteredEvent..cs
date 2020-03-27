using System;
using System.Security;

namespace UserRegistrationLibrary.Events.User
{
    internal class UserRegisteredEvent : UserEvent
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public UserRegisteredEvent(Guid userId, string email, string passwordHash) : base(userId)
        {
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
