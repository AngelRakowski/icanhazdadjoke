using System;
using System.Collections.Generic;
using System.Text;

namespace ICanHazDadJokeConsole.Model
{
    /* This class defines the settings used for the ICanHazDadJokeConsole app.
     */
    public class DadJokesSettings
    {
        public Dictionary<string, string> clientSettings;
        private  int _delayBetweenJokes = 10000;             // defined per requirements (10 seconds)
        public Dictionary<string, string> querySettings;
        private string _jokesPerPageLimit = @"30";          // defined per requirements (30 jokes per page)
        private int _shortJokeLimit = 10;
        private int _mediumJokeLimit = 20;
        private string _baseURL= @"https://icanhazdadjoke.com";

        public DadJokesSettings()
        {
            clientSettings = new Dictionary<string, string>();
            clientSettings.Add("Accept", "application/json");
            
            // Per the request from the documentation for ICanHazDadJoke API, I added a custom user-agent
            clientSettings.Add("User-Agent", @"My Library (https://github.com/AngelRakowski/icanhazdadjoke)");

        }

        public string BaseURL
        {
            get { return _baseURL; }
        } 
        public string SearchTerm { get; set; }
        
        public int ShortJokeLimit
        {
            get { return _shortJokeLimit; }
        }

        public int MediumJokeLimit
        {
            get { return _mediumJokeLimit; }
        }

        public string JokesPerPage
        {
            get { return _jokesPerPageLimit; }
        }

        public int JokesDelay
        {
            get { return _delayBetweenJokes; }
        }
    }
}
