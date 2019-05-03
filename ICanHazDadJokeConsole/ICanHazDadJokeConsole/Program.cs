using System;
using System.Threading.Tasks;
using System.Threading;

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

            // Only accept 1 or 2 from the user
            while (!input.Equals("1") && !input.Equals("2"))
            {
                input = PromptForValidInput();
            }
            
            int convertedInput = Convert.ToInt32(input);
            
            DadJokesService service = new DadJokesService();

            // We use a CancellationTokenSource to gracefully handle threading and closing of tasks
            // especially when the user enters a key to exit the program
            CancellationTokenSource source = new CancellationTokenSource(); 
            CancellationToken token = source.Token;

            Task t=null;
            Console.WriteLine("Kicking off task");
            if (convertedInput.Equals((int)DadJokesOption.SearchDadJokes))
            {
                Console.WriteLine("Enter in a search term.");
                service.JokesSettings.SearchTerm = Console.ReadLine();

                // kick off thread to Search Dad Jokes
                t = Task.Run(async () =>
                {
                    await service.SearchDadJokes();
                }, token);
            }
            else
            {
                // kick off thread to poll API every 10 seconds
                t = Task.Run(async () =>
                {
                    await service.RepeatDadJokes(source);
                }, token);
            }

            // We return here immediately after kicking off our thread 
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            
            // Wait for key input.
            Console.ReadKey();
           
            // send a Cancel request
            source.Cancel();

            Console.WriteLine("Cleaning up threads.  Please wait.");

            // wait for all tasks running to complete their execution
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

        static string PromptForValidInput()
        {
            Console.WriteLine("\nInvalid input.\n");
            DisplayInstructions();
            var input = Console.ReadLine();
            return input;
        }
    }
}
