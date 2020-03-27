using EventSourcing.Entities;
using System;

namespace UserRegistrationLibrary.Events.User
{
    internal class UserDeletedEvent : UserEvent, IProtectedDataDeletionEvent
    {
        public UserDeletedEvent(Guid userId) : base(userId)
        {

        }
    }
}
