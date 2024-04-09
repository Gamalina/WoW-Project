using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FetchDataApi_Test
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            string clientId = "33a49346d7034b2581ee752523b6db30";
            string clientSecret = "xItQKoZcyW8reQ6bBDF540Y3n6xm5e96";

            string realm = "bronze-dragonflight";
            string characterName = "aethiops";

            string accessToken = await GetAccessToken(clientId, clientSecret);

            if (!string.IsNullOrEmpty(accessToken))
            {

                string apiUrl = $"https://eu.api.blizzard.com/profile/wow/character/{realm}/{characterName}?namespace=profile-eu&locale=en_GB&access_token={accessToken}";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            dynamic characterInfo = JsonConvert.DeserializeObject(jsonResponse);

                            // Display relevant character information
                            Console.WriteLine($"Character: {characterInfo.name}");
                            Console.WriteLine($"Level: {characterInfo.level}");
                            Console.WriteLine($"Race: {characterInfo.race.name}");
                            Console.WriteLine($"Class: {characterInfo.character_class.name}");
                            Console.WriteLine($"Realm: {characterInfo.realm.name}");
                            Console.WriteLine($"Professions: {characterInfo.professions}");
                            Console.WriteLine($"Test: {characterInfo.ToString()}");
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            // Handle "Character Not Found" error
                            Console.WriteLine($"Character not found. {response.StatusCode}");
                        }
                        else
                        {
                            // Handle other API errors
                            Console.WriteLine($"Failed to retrieve character data. Status code: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to obtain access token.");
            }
        }

        static async Task<string> GetAccessToken(string clientId, string clientSecret)
        {
            
            string tokenUrl = $"https://eu.battle.net/oauth/token";
            string scope = "wow.profile";

            using (HttpClient client = new HttpClient())
            {
                var credentials = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", scope)
            });

                HttpResponseMessage response = await client.PostAsync(tokenUrl, credentials);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic tokenResponse = JsonConvert.DeserializeObject(jsonResponse);
                    return tokenResponse.access_token;
                }
                else
                {
                    Console.WriteLine($"Failed to obtain access token. Status code: {response.StatusCode}");
                    return null;
                }
            }
        }
    }
}
       
