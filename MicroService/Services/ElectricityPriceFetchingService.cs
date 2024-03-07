using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MicroService.Services
{
    public class ElectricityPriceFetchingService : BackgroundService
    {
        private readonly ILogger<ElectricityPriceFetchingService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ElectricityPriceFetchingService(ILogger<ElectricityPriceFetchingService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await FetchElectricityPricesAsync(stoppingToken);
                //Wait 1 hour to fetch again
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task FetchElectricityPricesAsync(CancellationToken stoppingToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.GetAsync(Constants.Constants.PorssisahkoUrl, stoppingToken);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Hinnat haettu: {content}");


                // Tässä kohtaa voitaisiin välittää data toiselle palvelulle tai tallentaa se
                var dataProcessingService = new ElectricityDataProcessingService(_httpClientFactory, _logger);
                await dataProcessingService.ProcessAndSaveElectricityDataAsync(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Virhe sähkön hintatietojen haussa");
            }
        }
    }
    public class ElectricityDataProcessingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ElectricityPriceFetchingService> _logger; // Muuta tyyppiä tähän

        public ElectricityDataProcessingService(IHttpClientFactory httpClientFactory, ILogger<ElectricityPriceFetchingService> logger) // Muuta tyyppiä tähän
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task ProcessAndSaveElectricityDataAsync(string electricityData)
        {
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                // Päivitä palvelimen osoite
                var apiUrl = "https://localhost:7277/api/ElectricityData"; // Oletetaan, että palvelin kuuntelee /api/electricitydata-reittiä

                // Voit säätää pyynnön sisältöä ja otsikoita tarpeen mukaan
                var content = new StringContent(electricityData, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Sähkön hintatiedot lähetetty toiselle palvelulle onnistuneesti.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Virhe sähkön hintatietojen lähettämisessä toiselle palvelulle.");
            }
        }

    }
}