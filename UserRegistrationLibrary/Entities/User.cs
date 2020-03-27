using EventSourcing;
using EventSourcing.Entities;
using System;
using System.Security;
using UserRegistrationLibrary.Events.User;

namespace UserRegistrationLibrary.Entities
{
    internal enum UserStatus
    {
        Blank,
        Created,
        Verified,
        Deleted
    }

    internal class User : EventStream
    {
        private Guid _userId;
        private string _email;
        private string _passwordHash;
        private string _unverifiedEmail;
        private UserStatus _status;

        public override Guid GetKey()
        {
            return _userId;
        }

        public void Register(Guid userId, string email, string passwordHash)
        {
            if (_status != UserStatus.Blank)
                throw new InvalidOperationException("User has already registered");

            Update(new UserRegisteredEvent(userId, email, passwordHash));
        }

        public void UpdateEmail(string newEmail)
        {
            if (_status != UserStatus.Verified)
                throw new InvalidOperationException("Cannot update email of user before the user has been verified");
            
            Update(new UpdateEmailEvent(_userId, newEmail));
        }

        public void EmailVerified(string verifiedEmail)
        {
            if (_unverifiedEmail != verifiedEmail)
                throw new InvalidOperationException("No corresponding email update request");
            
            Update(new EmailVerifiedEvent(_userId, verifiedEmail));
        }

        public void Delete()
        {
            if (_status == UserStatus.Blank || _status == UserStatus.Deleted)
                throw new InvalidOperationException("Cannot delete user that does not exist");
        }

        public override void Rehydrate(Event @event)
        {
            switch (@event)
            {
                case UserRegisteredEvent userRegistrationEvent:
                    _userId = userRegistrationEvent.UserId;
                    _unverifiedEmail = userRegistrationEvent.Email;
                    _passwordHash = userRegistrationEvent.PasswordHash;
                    _status = UserStatus.Created;
                    break;

                case UserDeletedEvent _:
                    _status = UserStatus.Deleted;
                    _email = default;
                    _unverifiedEmail = default;
                    _passwordHash = default;
                    break;

                case UpdateEmailEvent updateEmailEvent:
                    _unverifiedEmail = updateEmailEvent.NewEmail;
                    break;

                case EmailVerifiedEvent emailVerifiedEvent:
                    _email = emailVerifiedEvent.VerifiedEmail;
                    _status = UserStatus.Verified;
                    break;

                default:
                    // Event not relevant for entity
                    break;
            }
        }
    }
}
