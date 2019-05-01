using System;
using System.Collections.Generic;
using System.Text;

namespace ICanHazDadJokeConsole.Model
{
    // This class is the base class for DadJoke which is how the JSON response is returned from icanhazdadjoke.com/API.
    public class DadJoke
    {
        public string ID;
        public string Joke;
    }
}
