using EventSourcing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;
using System.Linq;
using System.Threading.Tasks;
using UserRegistrationLibrary.Services;

namespace UserRegistrationLibraryTests.Domain
{
    [TestClass]
    public class UserRegistrationQueryServiceTests
    {
        private static EventStoreMock EventStore;
        private static KeyStoreMock KeyStore;
        private static UserRegistrationCommandService UserRegistrationCommandService;
        private static UserRegistrationQueryService UserRegistrationQueryService;

        [TestInitialize]
        public void Initialize()
        {
            EventStore = new EventStoreMock();
            KeyStore = new KeyStoreMock();

            var eventRepository = new EventRepository(EventStore, KeyStore);
            UserRegistrationCommandService = new UserRegistrationCommandService(eventRepository);
            UserRegistrationQueryService = new UserRegistrationQueryService(eventRepository);
        }

        [TestMethod]
        public async Task GetUserByPartialEmail()
        {
            var email = "email";
            var emailQuery = "EMa";
            var passwordHash = "passwordHash";

            var resultsBefore = await UserRegistrationQueryService.SearchUser(emailQuery);

            Assert.IsFalse(resultsBefore.Contains(email));

            await UserRegistrationCommandService.RegisterUser(email, passwordHash);
            var userId = await UserRegistrationQueryService.GetUserId(email);
            await UserRegistrationCommandService.EmailVerified(userId, email);

            var resultsAfter = await UserRegistrationQueryService.SearchUser(emailQuery);

            Assert.IsTrue(resultsAfter.Contains(email));
        }
    }
}
