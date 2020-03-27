using EventSourcing;
using EventSourcing.Entities;
using EventSourcing.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using UserRegistrationLibrary.Events.UserRegistry;

namespace UserRegistrationLibrary.Entities
{
    public class UserRegistry : EventStream
    {
        private readonly Dictionary<string, Guid> _users;
        private readonly Dictionary<string, Guid> _reservedEmails;

        public UserRegistry()
        {
            _users = new Dictionary<string, Guid>();
            _reservedEmails = new Dictionary<string, Guid>();
        }

        #region Abstract implementation

        public override Guid GetKey()
        {
            return Guid.Empty;
        }

        public override void Rehydrate(Event @event)
        {
            switch (@event)
            {
                case UserAddedEvent userAddedEvent:
                    _users.Add(userAddedEvent.Email, userAddedEvent.UserId);
                    _reservedEmails.Add(userAddedEvent.Email, userAddedEvent.UserId);
                    break;

                case ReserveEmailEvent reserveEmailEvent:
                    _reservedEmails.Add(reserveEmailEvent.Email, reserveEmailEvent.UserId);
                    break;

                case EmailVerifiedEvent emailVerifiedEvent:
                    _users.Add(emailVerifiedEvent.Email, emailVerifiedEvent.UserId);
                    break;

                case UserRemovedEvent userRemovedEvent:
                    _users
                        .Where(kvp => kvp.Value == userRemovedEvent.UserId)
                        .Select(kvp => kvp.Key)
                        .ToList()
                        .ForEach(key => {
                            _users.Remove(key);
                            _reservedEmails.Remove(key);
                        });
                    break;

                default:
                    // Event not relevant for entity
                    break;
            }
        }

        #endregion

        #region Commands

        public void ReserveEmail(string email, Guid userId)
        {
            var lowerCaseEmail = email.ToLower();
            if (string.IsNullOrEmpty(lowerCaseEmail))
                throw new InvalidOperationException("Input parameters are not allowed to be empty");

            if (_reservedEmails.ContainsKey(lowerCaseEmail))
                throw new EmailInUseException("Email is already in use");

            Update(new ReserveEmailEvent(lowerCaseEmail, userId));
        }

        public void ReservedEmailVerified(Guid userId, string email)
        {
            var lowerCaseEmail = email.ToLower();
            if (string.IsNullOrEmpty(lowerCaseEmail) || userId == null || userId == Guid.Empty)
                throw new InvalidOperationException("Input parameters are not allowed to be empty");

            if (!_reservedEmails.ContainsKey(lowerCaseEmail) || _reservedEmails[lowerCaseEmail] != userId)
                throw new EmailInUseException("Email is not reserved for this user");

            Update(new EmailVerifiedEvent(userId, lowerCaseEmail));
        }

        public void Remove(Guid userId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new InvalidOperationException("Input parameters are not allowed to be empty");

            Update(new UserRemovedEvent(userId));
        }

        #endregion

        #region Queries

        public IEnumerable<string> RegisteredAccounts()
        {
            return _users.Select(kvp => kvp.Key).ToList();
        }

        public Guid GetUserId(string email)
        {
            var lowerCaseEmail = email.ToLower();
            if (string.IsNullOrEmpty(lowerCaseEmail))
                throw new InvalidOperationException("Input parameters are not allowed to be empty");

            if (!_users.ContainsKey(lowerCaseEmail) && !_reservedEmails.ContainsKey(lowerCaseEmail))
                throw new KeyNotFoundException("Input does not point to an existing account");

            return _users.ContainsKey(lowerCaseEmail) ? _users[lowerCaseEmail] : _reservedEmails[lowerCaseEmail];
        }

        #endregion
    }
}
