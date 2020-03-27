using System;
using System.Threading.Tasks;
using EventSourcing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;
using UserRegistrationLibrary.Events.User;
using UserRegistrationLibrary.Services;
using UserRegistrationLibrary;
using UserRegistrationLibrary.Entities;

namespace UserRegistrationLibraryTests.Domain
{
    [TestClass]
    public class UserRegistrationCommandServiceTests
    {
        private static EventRepository EventRepository;
        private static UserRegistrationCommandService UserRegistrationService;

        [TestInitialize]
        public void Initialize()
        {
            EventRepository = new EventRepository(new EventStoreMock(), new KeyStoreMock());
            UserRegistrationService = new UserRegistrationCommandService(EventRepository);
        }

        [TestMethod]
        public async Task UserRegistrationIsPersisted()
        {
            var email = "email";
            var passwordHash = "passwordHash";

            var userId = await UserRegistrationService.RegisterUser(email, passwordHash);

            var events = await EventRepository.Get<User>(userId);
            Assert.IsTrue(events.Exists<UserRegisteredEvent>(@event => @event.Email == email && @event.PasswordHash == passwordHash));
        }

        [TestMethod]
        public async Task CannotUpdateEmailBeforeRegistration()
        {
            var email = "email";

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => UserRegistrationService.UpdateEmail(Guid.NewGuid(), email));
        }
    }
}
