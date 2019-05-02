using System;
using System.Collections.Generic;
using System.Text;
using ICanHazDadJokeConsole.Model;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ICanHazDadJokeConsole
{
    public class DadJokesService
    {
        private DadJokesSettings _settings;
        private string _responseBody;

        public DadJokesService()
        {

            // set up the DadJokesSettings
            _settings = new DadJokesSettings();
        }

        public DadJokesSettings jokesSettings
        {
            get { return _settings; }
        }

        public void InitializeClient(HttpClient client)
        {

            foreach (var kv in _settings.clientSettings)
            {
                client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
            }
        }

       
        /*
* Function: FormatAndDisplayDadJokes(IList<DadJoke>)
* Per the requirements, if using a search term with the API, the joke must display with the search term
* highlighted in some way. In this case, I chose to capitalize all the letters in the word.  We do it with 
* all instances of the whole word using regular expressions.
*/
        public void FormatAndDisplayDadJokes(IList<DadJoke> jokes)
        {

            // We only want to match on the whole word
            string searchTerm = @"\b" + _settings.SearchTerm + @"\b";
            foreach (DadJoke dadJoke in jokes)
            {
                if (Regex.IsMatch(dadJoke.Joke, searchTerm))
                {
                    string formattedJoke = Regex.Replace(dadJoke.Joke, searchTerm, _settings.SearchTerm.ToUpper());

                    Console.WriteLine(formattedJoke);
                }

            }
        }

        /* Function: RepeatDadJokes(HttpClient, string, DadJokesSettings)
         * This function is called when Option 1 is selected from the UI.  It hits the API, displays the joke,
         * waits 10 seconds, and then repeats the process.
         * */
        public async Task RepeatDadJokes(CancellationTokenSource source)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    InitializeClient(client);

                    while(!source.IsCancellationRequested)
                    { 
                        _responseBody = await client.GetStringAsync(_settings.BaseURL);
                        DadJoke dadJoke = JsonConvert.DeserializeObject<DadJoke>(_responseBody);
                        Console.WriteLine(dadJoke.Joke);
                        await Task.Delay(_settings.JokesDelay);
                    }

                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }
            
        }

        /* Function: SearchDadJokes(HttpClient, string, DadJokesSettings)
        * This function is called when Option 2 is selected from the UI. It
        * builds a query with parameters in order to use the /search endpoint of the API.
        * It hits the API and returns a "page" of DadJokes given the limit set in the _settings.
        * It then binds the response to the DadJokes class and returns that for us to use in later functions.
        * */
        public async Task SearchDadJokes()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    InitializeClient(client);

                    string responseBody = "";
                    // Build the parameterized query then spawns a thread to return the responseBody as a string in an asyhonchronous operation. 
                    var builder   = new UriBuilder(_settings.BaseURL + "/search");
                    var query     = HttpUtility.ParseQueryString(builder.Query);
                    query.Add("term", _settings.SearchTerm);
                    query.Add("limit", _settings.JokesPerPage);
                    builder.Query = query.ToString();
                    string url    = builder.ToString();
                    responseBody  = await client.GetStringAsync(url);

                    // Bind the responseBody to our DadJokes object
                    DadJokes dadJokes = JsonConvert.DeserializeObject<DadJokes>(responseBody);

                    GroupDadJokesByLength(dadJokes);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }

        }


        /*
         * GroupDadJokesByLength(DadJokes dadJokes) is called by SearchDadJokes to 
         * sort the resulting Dad Jokes by length and display them by grouped length beginning with short jokes first.
         * Note: during  my testing I was not able to find jokes with a length under 20 so they 
         * all came out under Long Jokes
         */
        private void GroupDadJokesByLength(DadJokes dadJokes)
        {
            IList<DadJoke> shortJokes = new List<DadJoke>();
            IList<DadJoke> mediumJokes = new List<DadJoke>();
            IList<DadJoke> longJokes = new List<DadJoke>();

            foreach (DadJoke dadJoke in dadJokes.Results)
            {
                int wordCount = CountWords(dadJoke.Joke);

                if (wordCount < _settings.ShortJokeLimit)
                {
                    shortJokes.Add(dadJoke);
                }
                else if (wordCount > _settings.ShortJokeLimit && wordCount < _settings.MediumJokeLimit)
                {
                    mediumJokes.Add(dadJoke);
                }
                else
                {
                    longJokes.Add(dadJoke);
                }
            }

            if (shortJokes.Count != 0)
            {
                Console.WriteLine("*******************************************************");
                Console.WriteLine("Short jokes:");
                FormatAndDisplayDadJokes(shortJokes);
            }
            if (mediumJokes.Count != 0)
            {
                Console.WriteLine("*******************************************************");
                Console.WriteLine("Medium jokes:");
                FormatAndDisplayDadJokes(mediumJokes);
            }
            if (longJokes.Count != 0)
            {
                Console.WriteLine("*******************************************************");
                Console.WriteLine("Long jokes:");
                FormatAndDisplayDadJokes(longJokes);
            }
        }

        public int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
    }
}
