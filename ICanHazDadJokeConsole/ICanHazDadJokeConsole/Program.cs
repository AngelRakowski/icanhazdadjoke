using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
/*
 * Create an application using C#, .NET, and optionally ASP.NET that uses the “I can haz dad joke” api (https://icanhazdadjoke.com/api) to display jokes. You are welcome to use more technologies like Angular if you wish but it's not required.

There should be two modes the user can choose between:

1. Display a random joke every 10 seconds.

2. Accept a search term and display the first 30 jokes containing that term, with the matching term emphasized in some way (upper, bold, color, etc.) and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words)
*/

namespace ICanHazDadJokeConsole
{
    public class DadJoke
    {
        public string ID;
        public string Joke;
    }

    class Program
    {
        static readonly string  _baseURL = @"https://icanhazdadjoke.com";

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
                        while(true)
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

                        var searchTerm = Console.ReadLine();
                        var builder = new UriBuilder(_baseURL + "/search");
                        var query = HttpUtility.ParseQueryString(builder.Query);
                        query["term"] = searchTerm;
                        query["limit"] = "30";
                        builder.Query = query.ToString();
                        string url = builder.ToString();
                        responseBody = await client.GetStringAsync(url);

                        Console.WriteLine(responseBody);
                    }

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }


    }
}
