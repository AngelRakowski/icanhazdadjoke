using System;
using System.Collections.Generic;
using System.Text;

namespace ICanHazDadJokeConsole.Model
{
    // This class represents the information that we care about returned from the JSON response from icanhazdadjoke.com/API.
    // It contains a list of DadJokes returned that contain the Search term the user enters.

    public class DadJokes
    {
        public IList<DadJoke> Results;
        public string SearchTerm;
    }

}
