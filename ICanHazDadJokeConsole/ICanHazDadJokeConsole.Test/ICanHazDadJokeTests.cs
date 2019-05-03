using Microsoft.VisualStudio.TestTools.UnitTesting;
using ICanHazDadJokeConsole;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DadJoke = ICanHazDadJokeConsole.Model.DadJoke;
using DadJokes = ICanHazDadJokeConsole.Model.DadJokes;
using DadJokesSettings = ICanHazDadJokeConsole.Model.DadJokesSettings;


namespace ICanHazDadJokeConsole.Test
{
    [TestClass]
    public class ICanHazDadJokeTests
    {
        /// <summary>
        /// This test method hits the ICanHazDadJoke API and binds it to the DadJoke class.  We assert that 
        /// we get a response and that the bound DadJoke is not null.
        /// </summary>
        [TestMethod]
        public async Task GetASingleDadJokeTest()
        {
            string responseBody = "";

               using (HttpClient client = new HttpClient())
                {// set up the DadJokesSettings
                    DadJokesSettings settings = new DadJokesSettings();
                    foreach (var dadJokeKeyPair in settings.clientSettings)
                    {
                        client.DefaultRequestHeaders.Add(dadJokeKeyPair.Key, dadJokeKeyPair.Value);
                    }

                    responseBody = await client.GetStringAsync(settings.BaseURL);
                    Assert.IsTrue(responseBody.Length != 0);

                    DadJoke dadJoke = JsonConvert.DeserializeObject<DadJoke>(responseBody);
                    Assert.IsNotNull(dadJoke);
                }
          


        }

        /// <summary>
        /// This test method uses the Search function of the ICanHazDadJoke API, passes a searchTerm as a parameter
        /// to the query
        /// </summary>
        /// <Asserts>Response from API</Asserts>
        /// <Asserts>DadJokes search result is bound properly and not null</Asserts>
        /// 
        [TestMethod]
        public async Task SearchDadJokesBadInputTest()
        {
                using (HttpClient client = new HttpClient())
                {
                    DadJokesService service = new DadJokesService();

                    var settings = service.JokesSettings;
                    string responseBody = "";

                    foreach (var dadJokeKeyPair in settings.clientSettings)
                    {
                        client.DefaultRequestHeaders.Add(dadJokeKeyPair.Key, dadJokeKeyPair.Value);
                    }
                    settings.SearchTerm = "sdsdfadf";

                    // Build the parameterized query then spawns a thread to return the responseBody as a string in an asyhonchronous operation. 
                    var builder = new UriBuilder(settings.BaseURL + "/search");
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query.Add("term", settings.SearchTerm);
                    query.Add("limit", settings.JokesPerPage);
                    builder.Query = query.ToString();
                    string url = builder.ToString();
                    responseBody = await client.GetStringAsync(url);

                    Assert.IsTrue(responseBody.Length != 0);

                    DadJokes dadJokes = JsonConvert.DeserializeObject<DadJokes>(responseBody);
                    Assert.IsTrue(dadJokes.Results.Count==0);
                }
            

           
        }

        [TestMethod]
        public async Task SearchDadJokesInputTest()
        {
            using (HttpClient client = new HttpClient())
            {
                DadJokesService service = new DadJokesService();

                var settings = service.JokesSettings;
                string responseBody = "";

                foreach (var dadJokeKeyPair in settings.clientSettings)
                {
                    client.DefaultRequestHeaders.Add(dadJokeKeyPair.Key, dadJokeKeyPair.Value);
                }
                settings.SearchTerm = "have";

                // Build the parameterized query then spawns a thread to return the responseBody as a string in an asyhonchronous operation. 
                var builder = new UriBuilder(settings.BaseURL + "/search");
                var query = HttpUtility.ParseQueryString(builder.Query);
                query.Add("term", settings.SearchTerm);
                query.Add("limit", settings.JokesPerPage);
                builder.Query = query.ToString();
                string url = builder.ToString();
                responseBody = await client.GetStringAsync(url);

                Assert.IsTrue(responseBody.Length != 0);

                DadJokes dadJokes = JsonConvert.DeserializeObject<DadJokes>(responseBody);
                Assert.IsNotNull(dadJokes.Results);
            }



        }

        [TestMethod]
        public void GroupDadJokesTest()
        {
            using (HttpClient client = new HttpClient())
            {
               
                    DadJokesService service = new DadJokesService();
                    var settings = service.JokesSettings;
                  
                    DadJokes dadJokes = new DadJokes();
                    DadJoke joke1 = new DadJoke();
                    joke1.Joke = @"Why do bears have hairy coats? Fur protection.";
                    dadJokes.Results.Add(joke1);

                    DadJoke joke2 = new DadJoke();
                    joke2.Joke = @"What time did the man go to the dentist? Tooth hurt-y.";
                    dadJokes.Results.Add(joke2);

                    DadJoke joke3 = new DadJoke();
                    joke3.Joke = @"What did the left eye say to the right eye? Between us, something smells!";
                    dadJokes.Results.Add(joke3);

                    DadJoke joke4 = new DadJoke();
                    joke4.Joke = @"To the guy who invented zero... thanks for nothing.";
                    dadJokes.Results.Add(joke4);

                    Assert.IsNotNull(dadJokes);

                    IList<DadJoke> shortJokes  = new List<DadJoke>();
                    IList<DadJoke> mediumJokes = new List<DadJoke>();
                    IList<DadJoke> longJokes   = new List<DadJoke>();

                    foreach (DadJoke dadJoke in dadJokes.Results)
                    {
                        int wordCount = service.CountWords(dadJoke.Joke);

                        if (wordCount < settings.ShortJokeLimit)
                        {
                            shortJokes.Add(dadJoke);
                        }
                        else if (wordCount > settings.ShortJokeLimit && wordCount < settings.MediumJokeLimit)
                        {
                            mediumJokes.Add(dadJoke);
                        }
                        else
                        {
                            longJokes.Add(dadJoke);
                        }
                    }

                    Assert.IsTrue(shortJokes.Count != 0 || mediumJokes.Count != 0 || longJokes.Count != 0);
                

              
            }

        }

        [TestMethod]
        // Found on https://www.dotnetperls.com/word-count
        public void CountWords()
        {
            string s = @"The quick brown fox jumped over the lazy dog.  And then he ran over my mother.";
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            Assert.IsTrue(collection.Count == 16);
        }

    }
}
