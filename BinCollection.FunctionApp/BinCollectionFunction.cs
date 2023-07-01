using System.Net;
using BinCollection.FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace BinCollection.FunctionApp
{
    public class BinCollectionFunction
    {
        private readonly ILogger _logger;
        private readonly IBinCollectionService _binCollectionService;

        public BinCollectionFunction(ILoggerFactory loggerFactory, IBinCollectionService binCollectionService)
        {
            _logger = loggerFactory.CreateLogger<BinCollectionFunction>();
            _binCollectionService = binCollectionService;
        }

        [Function(nameof(RunBinCollectionService))]
        public async Task<HttpResponseData> RunBinCollectionService([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bincollection")] HttpRequestData req)
        {
            _logger.LogInformation("Running bin collection service");
            await _binCollectionService.RunAsync(false);
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function(nameof(RunBinCollectionScheduleCheck))]
        public async Task RunBinCollectionScheduleCheck([TimerTrigger("0 00 18 * * *")]
            TimerInfo timerInfo,
            FunctionContext context)
        {
            _logger.LogInformation("Running bin collection service schedule check");
            await _binCollectionService.RunAsync(true);
        }
    }
}
