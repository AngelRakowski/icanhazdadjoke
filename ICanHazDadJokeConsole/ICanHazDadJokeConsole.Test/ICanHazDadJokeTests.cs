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
        [TestMethod]
        public async Task GetASingleDadJokeTest()
        {
            using (HttpClient client = new HttpClient())
            {
                string responseBody = "";

                try
                {
                    // set up the DadJokesSettings
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

                catch(HttpRequestException)
                {
                    throw;
                }

            }
        }

        [TestMethod]
        public async Task SearchDadJokesTest()
        {

            using (HttpClient client = new HttpClient())
            {
                try
                {

                    DadJokesService service = new DadJokesService();

                    var settings = service.jokesSettings;
                    string responseBody = "";

                    foreach (var dadJokeKeyPair in settings.clientSettings)
                    {
                        client.DefaultRequestHeaders.Add(dadJokeKeyPair.Key, dadJokeKeyPair.Value);
                    }
                    settings.SearchTerm = "movie";

                    // Build the parameterized query then spawns a thread to return the responseBody as a string in an asyhonchronous operation. 
                    var builder = new UriBuilder(settings.BaseURL + "/search");
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query.Add("term", settings.SearchTerm);
                    query.Add("limit", settings.JokesPerPage);
                    builder.Query = query.ToString();
                    string url = builder.ToString();
                    responseBody = await client.GetStringAsync(url);

                    Assert.IsTrue(responseBody.Length != 0);

                    DadJokes dadJoke = JsonConvert.DeserializeObject<DadJokes>(responseBody);
                    Assert.IsNotNull(dadJoke);
                }

                catch (HttpRequestException)
                {
                    throw;
                }
            }
        }

        [TestMethod]
        public async Task GroupDadJokesTest()
        {
            using (HttpClient client = new HttpClient())
            {
                string responseBody = "";

                try
                {
                    DadJokesService service = new DadJokesService();
                    var settings = service.jokesSettings;
                    settings.SearchTerm = "the";

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
                    Assert.IsNotNull(dadJokes);

                    IList<DadJoke> shortJokes = new List<DadJoke>();
                    IList<DadJoke> mediumJokes = new List<DadJoke>();
                    IList<DadJoke> longJokes = new List<DadJoke>();

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

                    if (shortJokes.Count != 0)
                    {
                        Console.WriteLine("Short jokes");
                        service.FormatAndDisplayDadJokes(shortJokes);
                    }
                    if (mediumJokes.Count != 0)
                    {
                        Console.WriteLine("Medium jokes");
                        service.FormatAndDisplayDadJokes(mediumJokes);
                    }
                    if (longJokes.Count != 0)
                    {
                        Console.WriteLine("Long jokes");
                        service.FormatAndDisplayDadJokes(longJokes);
                    }

                    Assert.IsTrue(shortJokes.Count != 0 || mediumJokes.Count != 0 || longJokes.Count != 0);
                }

                catch (HttpRequestException)
                {
                    throw;
                }
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
