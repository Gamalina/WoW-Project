using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnknownNameController : ControllerBase
    {
        private readonly string apiUrl = "https://eu.api.blizzard.com/profile/wow/character/{0}/{1}?namespace=profile-eu&locale=en_GB&access_token={2}";
        private readonly ILogger<UnknownNameController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public UnknownNameController(ILogger<UnknownNameController> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        [HttpGet("{realm}/{characterName}")]
        public async Task<IActionResult> GetCharacter(string realm, string characterName)
        {
            var client = _clientFactory.CreateClient();

            var credentials = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _configuration["Blizzard:ClientId"]),
                new KeyValuePair<string, string>("client_secret", _configuration["Blizzard:ClientSecret"]),
                new KeyValuePair<string, string>("scope", "wow.profile")
            });

            HttpResponseMessage tokenResponse = await client.PostAsync(_configuration["Blizzard:TokenUrl"], credentials);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving access token from Blizzard API. Status code: {statusCode}", tokenResponse.StatusCode);
                return StatusCode((int)tokenResponse.StatusCode);
            }

            string jsonResponse = await tokenResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(jsonResponse))
            {
                _logger.LogError("The JSON Response is null or empty");
                return StatusCode(500);
            }

            dynamic tokenData = JsonConvert.DeserializeObject(jsonResponse);
            if (tokenData.access_token == null)
            {
                _logger.LogError("The access token is null.");
                return StatusCode(500);
            }
            string accessToken = tokenData.access_token;
            string accessTokenString = accessToken.ToString();

            HttpResponseMessage response = await client.GetAsync(string.Format(apiUrl, realm, characterName, accessTokenString));

            _logger.LogInformation("Blizzard API response: {response}", response);

            if (response.IsSuccessStatusCode)
            {
                string characterResponse = await response.Content.ReadAsStringAsync();
                return Ok(characterResponse);
            }
            else
            {
                _logger.LogError("Error retrieving character data from Blizzard API. Status code: {statusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode);
            }
        }

    }
}
