using CommandLine;
using EventSourcing;
using Mocks;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserRegistrationLibrary;
using UserRegistrationLibrary.Services;

namespace CLI
{
    class Program
    {
        private static bool ExitCommandGiven = false;

        private static UserRegistrationCommandService UserRegistrationCommandService;
        private static UserRegistrationQueryService UserRegistrationQueryService;

        [Verb("add", HelpText = "Register user")]
        public class AddOptions
        {
            [Option('e', "email", Required = true, HelpText = "Email")]
            public string Email { get; set; }

            [Option('p', "password", Required = true, HelpText = "Password")]
            public string Password { get; set; }
        }

        [Verb("verify", HelpText = "Verify email")]
        public class VerifyOptions
        {
            [Option('e', "email", Required = true, HelpText = "Email")]
            public string Email { get; set; }
        }

        [Verb("search", HelpText = "Search users based on their email address")]
        public class SearchOptions
        {
            [Option('q', "query", Required = true, HelpText = "Input query")]
            public string Query { get; set; }
        }

        [Verb("exit", HelpText = "Exit the application")]
        public class ExitOptions
        {
        }

        static void Main(string[] args)
        {
            var eventStore = new EventStoreMock();
            var keyStore = new KeyStoreMock();
            var repository = new EventRepository(eventStore, keyStore);
            UserRegistrationCommandService = new UserRegistrationCommandService(repository);
            UserRegistrationQueryService = new UserRegistrationQueryService(repository);

            while (!ExitCommandGiven)
            {
                Parser
                    .Default
                    .ParseArguments<AddOptions, VerifyOptions, SearchOptions, ExitOptions>(args)
                    .MapResult(
                        (AddOptions options) => Add(options).Result,
                        (VerifyOptions options) => Verify(options).Result,
                        (SearchOptions options) => Search(options).Result,
                        (ExitOptions _) => Exit(),
                        errors => 1);

                Console.WriteLine("Provide a command or type help for options");
                args = Console.ReadLine().Split(' ');
            }
        }

        static async Task<int> Add(AddOptions options)
        {
            try
            {
                await UserRegistrationCommandService.RegisterUser(options.Email, options.Password.ToSecureString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

            return 0;
        }

        static async Task<int> Verify(VerifyOptions options)
        {
            try
            {
                var userId = await UserRegistrationQueryService.GetUserId(options.Email);

                await UserRegistrationCommandService.EmailVerified(userId, options.Email);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

            return 0;
        }

        static async Task<int> Search(SearchOptions options)
        {
            try
            {
                var results = await UserRegistrationQueryService.SearchUser(options.Query);

                if (!results.Any())
                {
                    Console.WriteLine("No results found");
                }
                else
                {
                    Console.WriteLine($"{results.Count()} results found:");
                    foreach (var result in results)
                        Console.WriteLine(result);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

            return 0;
        }

        static int Exit()
        {
            ExitCommandGiven = true;

            return 0;
        }
    }
}
