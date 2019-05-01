using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DadJoke = ICanHazDadJokeConsole.Model.DadJoke;
using DadJokes = ICanHazDadJokeConsole.Model.DadJokes;
using DadJokesSettings = ICanHazDadJokeConsole.Model.DadJokesSettings;



/* REQUIREMENTS
 * 
 * Create an application using C#, .NET, and optionally ASP.NET that uses the “I can haz dad joke” api (https://icanhazdadjoke.com/api) to display jokes. You are welcome to use more technologies like Angular if you wish but it's not required.

There should be two modes the user can choose between:

1. Display a random joke every 10 seconds.

2. Accept a search term and display the first 30 jokes containing that term, 
with the matching term emphasized in some way (upper, bold, color, etc.) 
and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words)
*/

namespace ICanHazDadJokeConsole
{
    public class Controller
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the ICanHazDadJoke.com API.");
            DisplayInstructions();

            var input = Console.ReadLine();

            int convertedInput = Convert.ToInt32(input);

            // If the user enters in an input other than 1 or 2, it will keep asking them to re-enter it
            while (convertedInput != 1 && convertedInput != 2)
            {
                Console.WriteLine("\nInvalid input.\n");
                DisplayInstructions();
                input = Console.ReadLine();
                convertedInput = Convert.ToInt32(input);
            }

            // Spawn a thread to GetDadJokes
            Task.Run(async () =>
            {
                 await GetDadJokes(convertedInput);
            }).GetAwaiter().GetResult();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void DisplayInstructions()
        {
            Console.WriteLine("Please enter 1 to display a random joke every 10 seconds.");
            Console.WriteLine("Please enter 2 to display the first 30 jokes containing a search term and grouped by length.");
        }

        public static async Task GetDadJokes(int input)
        {
            // Code pulled from: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netframework-4.8
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    string responseBody = "";

                    // set up the DadJokesSettings
                    DadJokesSettings settings = new DadJokesSettings();
                    foreach (var dadJokeKeyPair in settings._clientSettings)
                    {
                        client.DefaultRequestHeaders.Add(dadJokeKeyPair.Key, dadJokeKeyPair.Value);
                    }

                    if (input == 1)
                    {
                        while (true)
                        {
                            await RepeatDadJokes(client, responseBody, settings);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nEnter in a search term.");
                        Model.DadJokesSettings._searchTerm = Console.ReadLine();

                        DadJokes resultingDadJokes = await SearchDadJokes(client, responseBody, settings);

                        GroupDadJokesByLength(resultingDadJokes);

                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }

        public static void FormatAndDisplayDadJokes(IList<DadJoke> jokes)
        {
            string searchTerm = @"\b" + Model.DadJokesSettings._searchTerm + @"\b";
            foreach (DadJoke dadJoke in jokes)
            {
                if (Regex.IsMatch(dadJoke.Joke, searchTerm))
                {
                    string formattedJoke = Regex.Replace(dadJoke.Joke, searchTerm, Model.DadJokesSettings._searchTerm.ToUpper());

                    Console.WriteLine(formattedJoke);
                }
           
            }
        }

        public static async Task RepeatDadJokes(HttpClient client, string responseBody, DadJokesSettings settings)
        {
            responseBody = await client.GetStringAsync(Model.DadJokesSettings._baseURL);
            DadJoke dadJoke = JsonConvert.DeserializeObject<DadJoke>(responseBody);
            Console.WriteLine(dadJoke.Joke);
            await Task.Delay(settings._delayBetweenJokes);
        }

        public static async Task<DadJokes>  SearchDadJokes(HttpClient client, string responseBody, DadJokesSettings settings)
        {
            // Build the parameterized query then spawns a thread to return the responseBody as a string in an asyhonchronous operation. 
            var builder = new UriBuilder(Model.DadJokesSettings._baseURL + "/search");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query.Add("term", Model.DadJokesSettings._searchTerm);
            query.Add("limit", settings._jokesPerPageLimit);
            builder.Query = query.ToString();
            string url = builder.ToString();
            responseBody = await client.GetStringAsync(url);

            // Map the responseBody to our DadJokes object
            DadJokes dadJokes = JsonConvert.DeserializeObject<DadJokes>(responseBody);
            return dadJokes;
        }

        public static void  GroupDadJokesByLength(DadJokes dadJokes)
        {
            IList<DadJoke> shortJokes = new List<DadJoke>();
            IList<DadJoke> mediumJokes = new List<DadJoke>();
            IList<DadJoke> longJokes = new List<DadJoke>();

            foreach (DadJoke dadJoke in dadJokes.Results)
            {
                if (dadJoke.Joke.Length < 10)
                {
                    shortJokes.Add(dadJoke);
                }
                else if (dadJoke.Joke.Length > 10 && dadJoke.Joke.Length < 20)
                {
                    mediumJokes.Add(dadJoke);
                }
                else if (dadJoke.Joke.Length >= 20)
                {
                    longJokes.Add(dadJoke);
                }
            }

            if (shortJokes.Count != 0)
            {
                Console.WriteLine("Short jokes");
                FormatAndDisplayDadJokes(shortJokes);
            }
            if (mediumJokes.Count != 0)
            {
                Console.WriteLine("Medium jokes");
                FormatAndDisplayDadJokes(mediumJokes);
            }
            if (longJokes.Count != 0)
            {
                Console.WriteLine("Long jokes");
                FormatAndDisplayDadJokes(longJokes);
            }
        }

    }

}
