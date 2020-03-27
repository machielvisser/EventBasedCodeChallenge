using System;

namespace UserRegistrationLibrary.Events.User
{
    internal class EmailVerifiedEvent : UserEvent
    {
        public string VerifiedEmail { get; set; }

        public EmailVerifiedEvent(Guid userId, string verifiedEmail) : base(userId)
        {
            VerifiedEmail = verifiedEmail;
        }
    }
}
