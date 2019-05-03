using System;
using System.Collections.Generic;
using System.Text;

namespace ICanHazDadJokeConsole.Model
{
    // This class represents the information that we care about returned from the JSON response 
    // from the icanhazdadjoke.com.
    // It contains a list of DadJokes returned that contain the Search term the user enters.

    public class DadJokes
    {
        public DadJokes()
        {
            Results = new List<DadJoke>();
        }

        // List of DadJokes returned from a search call to the API
        public IList<DadJoke> Results { get; set; }

        // Http status code
        public string Status { get; set; }
       
    }

}
