using BinCollection.FunctionApp.Clients;
using BinCollection.FunctionApp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BinCollection.FunctionApp.Services
{
    public class BinCollectionService : IBinCollectionService
    {
        private readonly IBinCollectionHttpClient _binCollectionHttpClient;
        private readonly ITextService _textService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BinCollectionService> _logger;

        public BinCollectionService(
            IBinCollectionHttpClient binCollectionHttpClient,
            ITextService textService,
            IConfiguration configuration,
            ILogger<BinCollectionService> logger)
        {
            _binCollectionHttpClient = binCollectionHttpClient;
            _textService = textService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task RunAsync(bool onlySendIfTomorrow = false)
        {
            var uprn = _configuration.GetValue<string>("Uprn");
            var binCollectionClientResponse = await _binCollectionHttpClient.GetBinCollectionDatesAsync(uprn);

            if (!binCollectionClientResponse.Success)
            {
                _logger.LogError("Bin collection http client was not successful");
                return;
            }

            var binCollections = binCollectionClientResponse.Collections;

            var nextBinCollections = binCollections
                                        .GroupBy(c => c.Next)
                                        .OrderBy(g => g.Key)
                                        .First()
                                        .Select(x => x)
                                        .ToList();

            var nextBinCollectionDate = nextBinCollections.First().Next;

            if (!(DateTime.Compare(nextBinCollectionDate.Date, DateTime.Now.Date.AddDays(1)) == 0) && onlySendIfTomorrow)
            {
                return;
            }

            var nextBinCollectionTypes = nextBinCollections.Select(c => c.TaskType.ToBinCollectionText()).ToList();

            var nextBinCollectionText = ConstructBinCollectionTypeText(nextBinCollectionTypes);

            var binCollectionMessage = $"Hey Matt! The next collection is {nextBinCollectionDate:dd/MM/yyyy}, it's for {nextBinCollectionText}";
            
            await _textService.SendText(binCollectionMessage);
        }

        private string ConstructBinCollectionTypeText(List<string> binCollectionTypes)
        {
            var binCollectionText =
                string.Join(", ", binCollectionTypes)
                      .TrimEnd();

            var lastComma = binCollectionText.LastIndexOf(',');

            if (lastComma == -1)
            {
                return binCollectionText;
            }

            return binCollectionText
                    .Remove(lastComma, 1)
                    .Insert(lastComma, " and");
        }
    }
}
