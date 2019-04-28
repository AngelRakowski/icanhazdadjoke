using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;

/*
 * Create an application using C#, .NET, and optionally ASP.NET that uses the “I can haz dad joke” api (https://icanhazdadjoke.com/api) to display jokes. You are welcome to use more technologies like Angular if you wish but it's not required.

There should be two modes the user can choose between:

1. Display a random joke every 10 seconds.

2. Accept a search term and display the first 30 jokes containing that term, 
with the matching term emphasized in some way (upper, bold, color, etc.) 
and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words)
*/

namespace ICanHazDadJokeConsole
{
    public class DadJoke
    {
        public string ID;
        public string Joke;
    }

    public class DadJokes
    {
        public IList<DadJoke> Results;
        public string SearchTerm;
    }

    class Program
    {
        static readonly string  _baseURL = @"https://icanhazdadjoke.com";
        static string _searchTerm;

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the ICanHazDadJoke.com API.");
            DisplayInstructions();

            var input = Console.ReadLine();

            int convertedInput = Convert.ToInt32(input);
            while (convertedInput != 1 && convertedInput != 2)
            {
                Console.WriteLine("\nInvalid input.\n");
                DisplayInstructions();
                input = Console.ReadLine();
                convertedInput = Convert.ToInt32(input);
            }

            Task.Run(async () =>
            {
                 await GetDadJokes(convertedInput);
            }).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        static void DisplayInstructions()
        {
            Console.WriteLine("Please enter 1 to display a random joke every 10 seconds.");
            Console.WriteLine("Please enter 2 to display the first 30 jokes containing a search term and grouped by length.");
        }

        static async Task GetDadJokes(int input)
        {
            // Code pulled from: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netframework-4.8
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    string responseBody = "";

                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    // client.DefaultRequestHeaders.Add("User-Agent", "github.com/AngelRakowski/icanhazdadjoke");

                    if (input == 1)
                    {
                        while (true)
                        {
                            responseBody = await client.GetStringAsync(_baseURL);
                            DadJoke dadJoke = JsonConvert.DeserializeObject<DadJoke>(responseBody);
                            Console.WriteLine(dadJoke.Joke);
                            await Task.Delay(10000);
                        }



                    }
                    else
                    {
                        Console.WriteLine("\nEnter in a search term:");

                        _searchTerm = Console.ReadLine();
                        var builder = new UriBuilder(_baseURL + "/search");
                        var query      = HttpUtility.ParseQueryString(builder.Query);
                        query["term"]  = _searchTerm;
                        query["limit"] = "30";
                        builder.Query  = query.ToString();
                        string url     = builder.ToString();
                        responseBody   = await client.GetStringAsync(url);

                        DadJokes dadJokes = JsonConvert.DeserializeObject<DadJokes>(responseBody);
                        IList<DadJoke> shortJokes  = new List<DadJoke>();
                        IList<DadJoke> mediumJokes = new List<DadJoke>();
                        IList<DadJoke> longJokes   = new List<DadJoke>();

                        foreach (DadJoke dadJoke in dadJokes.Results)
                        {
                            if (dadJoke.Joke.Length <10)
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
                            FormatAndDisplayDadJokes(shortJokes);
                        }
                        if (mediumJokes.Count != 0)
                        {
                            FormatAndDisplayDadJokes(mediumJokes);
                        }
                        if (longJokes.Count != 0)
                        {
                            FormatAndDisplayDadJokes(longJokes);
                        }
                    }

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }

        static void FormatAndDisplayDadJokes(IList<DadJoke> jokes)
        {
            foreach (DadJoke dadJoke in jokes)
            {
                int start = dadJoke.Joke.IndexOf(_searchTerm);
                string formattedJoke = dadJoke.Joke.Insert(start + _searchTerm.Length, "<b>");
                
                Console.WriteLine(formattedJoke = formattedJoke.Insert(start, "<b>"));
            }
        }
    }
}
