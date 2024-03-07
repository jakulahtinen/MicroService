using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Text;
using System.Text.Json;

namespace MicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricityController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ElectricityController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("GetSahko")]
        public async Task<IActionResult> GetLatestPrices()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.Constants.PorssisahkoUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                await SendDataToOtherService(content);

                return Ok(content);
            }
            catch (HttpRequestException e)
            {
                // Log error (e.Message) or handle accordingly
                return StatusCode(500, "Virhe datan haussa: " + e.Message);
            }
        }

        private async Task SendDataToOtherService(string content)
        {
            var url = Constants.Constants.ElectrictyDataUrl; // Osoite endpointillesi
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, stringContent);
                response.EnsureSuccessStatusCode(); // Heittää poikkeuksen, jos status-koodi ei ilmaise onnistumista

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Server response: {responseContent}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error sending request: {e.Message}");
            }
        }
    }
}