using BinCollection.FunctionApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System.Text;

namespace BinCollection.FunctionApp.Clients
{
    public class BinCollectionHttpClient : IBinCollectionHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BinCollectionHttpClient> _logger;

        public BinCollectionHttpClient(IHttpClientFactory httpClientFactory, ILogger<BinCollectionHttpClient> logger)
        {
            _httpClient = httpClientFactory.CreateClient("BinCollectionHttpClient");
            _logger = logger;
        }

        public async Task<(bool Success, List<Models.BinCollection> Collections)> GetBinCollectionDatesAsync(string uprn)
        {
            try
            {
                var binCollectionRequest = new BinCollectionRequest { uprn = uprn, noticeBoard = "default" };
                var content = JsonConvert.SerializeObject(binCollectionRequest);

                var policy = Policy
                    .Handle<Exception>()
                    .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                    .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(15));

                var response = await policy.ExecuteAndCaptureAsync(async () =>
                {
                    return await _httpClient.PostAsync(_httpClient.BaseAddress, new StringContent(content, Encoding.UTF8, "application/json"));
                });
                
                if (response.Outcome is OutcomeType.Failure && response.FinalException is null)
                {
                    _logger.LogError($"API call failed with status code {response.FinalHandledResult.StatusCode} and error {await response?.FinalHandledResult?.Content?.ReadAsStringAsync()}");
                    return (false, new List<Models.BinCollection>());
                }

                if (response.Outcome is OutcomeType.Failure && response.FinalException is not null)
                {
                    _logger.LogError(response.FinalException, $"API call failed with status code {response.FinalHandledResult.StatusCode}");
                    return (false, new List<Models.BinCollection>());
                }

                var result = response.Result;

                var body = await result.Content.ReadAsStringAsync();
                var binCollectionResponse = JsonConvert.DeserializeObject<BinCollectionResponse>(body);

                if (binCollectionResponse?.data is null || !binCollectionResponse.data.SelectMany(x => x.BinCollections).Any())
                {
                    _logger.LogWarning("No bin collections were found from the API");
                    return (false, new List<Models.BinCollection>());
                }

                return (true, binCollectionResponse.data.SelectMany(x => x.BinCollections).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error occured fetching bin collection data");
                return (false, new List<Models.BinCollection>());
            }
        }
    }
}
