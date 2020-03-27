using EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRegistrationLibrary.Entities;

namespace UserRegistrationLibrary.Services
{
    public class UserRegistrationQueryService
    {
        private readonly QueryProcessor queryProcessor;

        public UserRegistrationQueryService(EventRepository eventRepository)
        {
            queryProcessor = new QueryProcessor(eventRepository);
        }

        public async Task<IEnumerable<string>> SearchUser(string emailQuery)
        {
            return await queryProcessor.Query(default, (UserRegistry userRegistry) => EmailQuery(userRegistry, emailQuery));
        }

        public async Task<Guid> GetUserId(string email)
        {
            return await queryProcessor.Query(default, (UserRegistry userRegistry) => userRegistry.GetUserId(email));
        }

        private IEnumerable<string> EmailQuery(UserRegistry userRegistry, string emailQuery)
        {
            var lowerCaseEmailQuery = emailQuery.ToLower();

            return userRegistry
                .RegisteredAccounts()
                .Where(email => email.Contains(lowerCaseEmailQuery))
                .ToList();
        }
    }
}
