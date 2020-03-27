using EventSourcing;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UserRegistrationLibrary.Entities;

[assembly: InternalsVisibleTo("UserRegistrationLibraryTests")]
namespace UserRegistrationLibrary.Services
{
    public class UserRegistrationCommandService
    {
        private readonly CommandProcessor _commandProcessor;

        public UserRegistrationCommandService(EventRepository eventRepository)
        {
            _commandProcessor = new CommandProcessor(eventRepository);
        }

        public async Task<Guid> RegisterUser(string email, string passwordHash)
        {
            var userId = Guid.NewGuid();

            await _commandProcessor.Process(Guid.Empty, (UserRegistry userRegistry) => userRegistry.ReserveEmail(email, userId));
            await _commandProcessor.Process(userId, (User user) => user.Register(userId, email, passwordHash));

            return userId;
        }

        public async Task UpdateEmail(Guid userId, string newEmail)
        {
            await _commandProcessor.Process(Guid.Empty, (UserRegistry userRegistry) => userRegistry.ReserveEmail(newEmail, userId));
            await _commandProcessor.Process(userId, (User user) => user.UpdateEmail(newEmail));
        }

        public async Task EmailVerified(Guid userId, string email)
        {
            await _commandProcessor.Process(userId, (User user) => user.EmailVerified(email));
            await _commandProcessor.Process(Guid.Empty, (UserRegistry userRegistry) => userRegistry.ReservedEmailVerified(userId, email));
        }

        public async Task DeleteUser(Guid userId)
        {
            await _commandProcessor.Process(userId, (User user) => user.Delete());
            await _commandProcessor.Process(Guid.Empty, (UserRegistry userRegistry) => userRegistry.Remove(userId));
        }
    }
}
