using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace BinCollection.FunctionApp.Middleware
{
    public class ApiKeyMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly string timerTriggerEntryPoint = "BinCollection.FunctionApp.BinCollectionFunction.RunBinCollectionScheduleCheck";

        public ApiKeyMiddleware(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (context.FunctionDefinition.EntryPoint == timerTriggerEntryPoint)
            {
                await next(context);
                return;
            }

            var apiKey = _configuration.GetValue<string>("X-BinCollection-Api-Key");
            if (context.BindingContext.BindingData.TryGetValue("X-BinCollection-Api-Key", out var apiHeaderValue))
            {
                if (apiHeaderValue as string != apiKey)
                {
                    await CreateReturnResponse(context, HttpStatusCode.Unauthorized);
                }
                else
                {
                    await next(context);
                }
            }
            else
            {
                await CreateReturnResponse(context, HttpStatusCode.Forbidden);
            }
        }

        private async Task CreateReturnResponse(FunctionContext context, HttpStatusCode httpStatusCode)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();
            var response = httpRequestData?.CreateResponse();
            response.StatusCode = httpStatusCode;
            var invocationResult = context.GetInvocationResult();
            invocationResult.Value = response;
        }
    }
}
