using Newtonsoft.Json;
using RestSharp;

Console.WriteLine("Test");

string GetAccessToken(string clientId, string clientSecret)
{
    var client = new RestClient("https://eu.battle.net/oauth/token");
    var request = new RestRequest("Method.Post");
    request.AddHeader("cache-control", "no-cache");
    request.AddHeader("content-type", "application/x-www-form-urlencoded");
    request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}", ParameterType.RequestBody);
    RestResponse response = client.Execute(request);

    var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content);

    return tokenResponse.access_token;
}


public class AccessTokenResponse
{
    public string access_token { get; set; }
}



Console.WriteLine(access);
