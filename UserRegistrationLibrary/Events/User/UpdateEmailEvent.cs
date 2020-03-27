using System;

namespace UserRegistrationLibrary.Events.User
{
    internal class UpdateEmailEvent : UserEvent
    {
        public string NewEmail { get; set; }

        public UpdateEmailEvent(Guid userId, string newEmail) : base(userId)
        {
            NewEmail = newEmail;
        }
    }
}
