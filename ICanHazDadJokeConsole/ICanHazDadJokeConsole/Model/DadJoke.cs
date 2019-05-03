﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICanHazDadJokeConsole.Model
{
    // This class is the base class for DadJoke which is how the JSON response is returned from the icanhazdadjoke.com API.
    public class DadJoke
    {
        public string Joke
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }
    }
}
