using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DadJoke = ICanHazDadJokeConsole.Model.DadJoke;
using DadJokes = ICanHazDadJokeConsole.Model.DadJokes;
using DadJokesSettings = ICanHazDadJokeConsole.Model.DadJokesSettings;



/* REQUIREMENTS
 * 
 * Create an application using C#, .NET, and optionally ASP.NET that uses the “I can haz dad joke” api 
 * (https://icanhazdadjoke.com/api) to display jokes.
 * You are welcome to use more technologies like Angular if you wish but it's not required.

There should be two modes the user can choose between:

1. Display a random joke every 10 seconds.

2. Accept a search term and display the first 30 jokes containing that term, 
with the matching term emphasized in some way (upper, bold, color, etc.) 
and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words)
**/

namespace ICanHazDadJokeConsole
{
    public enum DadJokesOption
    {
        RepeatDadJokes = 1,
        SearchDadJokes
    }

    public class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the ICanHazDadJoke.com API.");
            DisplayInstructions();

            var input = Console.ReadLine();

            int convertedInput = Convert.ToInt32(input);

            // If the user enters in an input other than 1 or 2, it will keep asking them to re-enter it
            while ( !convertedInput.Equals(1) && !convertedInput.Equals(2))
            {
                Console.WriteLine("\nInvalid input.\n");
                DisplayInstructions();
                input = Console.ReadLine();
                convertedInput = Convert.ToInt32(input);
            }


            DadJokesService service = new DadJokesService();
            CancellationTokenSource source = new CancellationTokenSource(); 

            CancellationToken token = source.Token;

            Task t=null;
            if (convertedInput.Equals(2))
            {
                Console.WriteLine("Enter in a search term.");

                service.jokesSettings.SearchTerm = Console.ReadLine();
                t = Task.Run(async () =>
                {
                    await service.SearchDadJokes();
                }, token);
            }
            else
            {   t = Task.Run(async () =>
                {
                    await service.RepeatDadJokes(source);
                }, token);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
           

            source.Cancel();
            Console.WriteLine("Cleaning up threads.  Please wait.");
            Task.WaitAll(t);
        }

        /*
         * Helper function: DisplayInstructions()
         * Displays the instructions to the user.
         * */
        static void DisplayInstructions()
        {
            Console.WriteLine("Please enter 1 to display a random joke every 10 seconds.");
            Console.WriteLine("Please enter 2 to display the first 30 jokes containing a search term and grouped by length.");
        }

       

    }
}
