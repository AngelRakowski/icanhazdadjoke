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
    public class UnitTest1
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
                    foreach (var dadJokeKeyPair in settings._clientSettings)
                    {
                        client.DefaultRequestHeaders.Add(dadJokeKeyPair.Key, dadJokeKeyPair.Value);
                    }

                    responseBody = await client.GetStringAsync(Model.DadJokesSettings._baseURL);
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
                string responseBody = "";

                try
                {
                    // set up the DadJokesSettings
                    DadJokesSettings settings = new DadJokesSettings();
                    foreach (var dadJokeKeyPair in settings._clientSettings)
                    {
                        client.DefaultRequestHeaders.Add(dadJokeKeyPair.Key, dadJokeKeyPair.Value);
                    }
                    DadJokesSettings._searchTerm = "movie";

                    // Build the parameterized query then spawns a thread to return the responseBody as a string in an asyhonchronous operation. 
                    var builder = new UriBuilder(DadJokesSettings._baseURL + "/search");
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query.Add("term", DadJokesSettings._searchTerm);
                    query.Add("limit", settings._jokesPerPageLimit);
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
                    // set up the DadJokesSettings
                    DadJokesSettings settings = new DadJokesSettings();
                    foreach (var dadJokeKeyPair in settings._clientSettings)
                    {
                        client.DefaultRequestHeaders.Add(dadJokeKeyPair.Key, dadJokeKeyPair.Value);
                    }
                    DadJokesSettings._searchTerm = "the";

                    // Build the parameterized query then spawns a thread to return the responseBody as a string in an asyhonchronous operation. 
                    var builder = new UriBuilder(Model.DadJokesSettings._baseURL + "/search");
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query.Add("term", Model.DadJokesSettings._searchTerm);
                    query.Add("limit", settings._jokesPerPageLimit);
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
                        if (dadJoke.Joke.Length < 10)
                        {
                            shortJokes.Add(dadJoke);
                        }
                        else if (dadJoke.Joke.Length > 10 && dadJoke.Joke.Length < 20)
                        {
                            mediumJokes.Add(dadJoke);
                        }
                        else if (dadJoke.Joke.Length >= 20)
                        {
                            longJokes.Add(dadJoke);
                        }
                    }

                    if (shortJokes.Count != 0)
                    {
                        Console.WriteLine("Short jokes");
                        Controller.FormatAndDisplayDadJokes(shortJokes);
                    }
                    if (mediumJokes.Count != 0)
                    {
                        Console.WriteLine("Medium jokes");
                        Controller.FormatAndDisplayDadJokes(mediumJokes);
                    }
                    if (longJokes.Count != 0)
                    {
                        Console.WriteLine("Long jokes");
                        Controller.FormatAndDisplayDadJokes(longJokes);
                    }

                    Assert.IsTrue(shortJokes.Count != 0 || mediumJokes.Count != 0 || longJokes.Count  != 0);
                }

                catch (HttpRequestException)
                {
                    throw;
                }

            }
        }
    }
}
