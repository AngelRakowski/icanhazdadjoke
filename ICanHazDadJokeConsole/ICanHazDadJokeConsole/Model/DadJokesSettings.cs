using System;
using System.Collections.Generic;
using System.Text;

namespace ICanHazDadJokeConsole.Model
{
    /* This class defines the settings used for the ICanHazDadJokeConsole app.
     */
    public class DadJokesSettings
    {
        public Dictionary<string, string> _clientSettings;
        public readonly int _delayBetweenJokes = 10000;             // defined per requirements (10 seconds)
        public Dictionary<string, string> _querySettings;
        public readonly string _jokesPerPageLimit = @"30";          // defined per requirements (30 jokes per page)
        public readonly int _shortJokeLimit = 10;
        public readonly int _mediumJokeLimit = 20;


        public DadJokesSettings()
        {
            _clientSettings = new Dictionary<string, string>();
            _clientSettings.Add("Accept", "application/json");
            
            // Per the request from the documentation for ICanHazDadJoke API, I added a custom user-agent
            _clientSettings.Add("User-Agent", @"My Library (https://github.com/AngelRakowski/icanhazdadjoke)");
        }

        public static readonly string _baseURL = @"https://icanhazdadjoke.com";
        public static string _searchTerm;
    }
}
