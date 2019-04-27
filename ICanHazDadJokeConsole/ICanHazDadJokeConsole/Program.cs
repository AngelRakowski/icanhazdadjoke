using System;

namespace ICanHazDadJokeConsole
{
    class Program
    {
        static void Main(string[] args)
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

        }

        static void DisplayInstructions()
        {
            Console.WriteLine("Please enter 1 to display a random joke every 10 seconds.");
            Console.WriteLine("Please enter 2 to display the first 30 jokes containing a search term and grouped by length.");
        }
    }
}
