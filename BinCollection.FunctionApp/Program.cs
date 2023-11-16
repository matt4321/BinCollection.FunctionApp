using BinCollection.FunctionApp.Clients;
using BinCollection.FunctionApp.Middleware;
using BinCollection.FunctionApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(x => x.UseMiddleware<ApiKeyMiddleware>())
    .ConfigureAppConfiguration((h, c) => 
    {
        c.AddJsonFile("appsettings.json", optional: true);
        c.AddJsonFile($"local.settings.json", optional: true);
    })
    .ConfigureLogging(x => x.AddConsole());

hostBuilder.ConfigureServices(serviceCollection =>
{
    serviceCollection.AddTransient<IBinCollectionHttpClient, BinCollectionHttpClient>();

    serviceCollection.AddTransient<ITextService, TextService>(sp =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var logger = sp.GetRequiredService<ILogger<TextService>>();
        var accountSid = configuration.GetValue<string>("Twilio:AccountSid");
        var authToken = configuration.GetValue<string>("Twilio:AuthToken");
        var toPhoneNumber = configuration.GetValue<string>("ToPhoneNumber");
        return new TextService(accountSid, authToken, toPhoneNumber, logger);
    });

    serviceCollection.AddTransient<IBinCollectionService, BinCollectionService>();

    serviceCollection.AddHttpClient("BinCollectionHttpClient", client =>
    {
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36");
        client.BaseAddress = new Uri("https://gis.stalbans.gov.uk/NoticeBoard9/VeoliaProxy.NoticeBoard.asmx/GetServicesByUprnAndNoticeBoard");
    });
});

var host = hostBuilder.Build();
await host.RunAsync();
