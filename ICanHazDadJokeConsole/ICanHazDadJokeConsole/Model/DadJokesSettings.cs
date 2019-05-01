using System;
using System.Collections.Generic;
using System.Text;

namespace ICanHazDadJokeConsole.Model
{
    public class DadJokesSettings
    {
        public Dictionary<string, string> _clientSettings;
        public readonly int _delayBetweenJokes = 10000;
        public Dictionary<string, string> _querySettings;
        public readonly string _jokesPerPageLimit = @"30";
        public DadJokesSettings()
        {
            _clientSettings = new Dictionary<string, string>();
            _clientSettings.Add("Accept", "application/json");
            _clientSettings.Add("User-Agent", @"My Library (https://github.com/AngelRakowski/icanhazdadjoke)");
        }


        public static readonly string _baseURL = @"https://icanhazdadjoke.com";
        public static string _searchTerm;
    }
}
