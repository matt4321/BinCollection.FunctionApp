using BinCollection.FunctionApp.Models;

namespace BinCollection.FunctionApp.Clients
{
    public interface IBinCollectionHttpClient
    {
        Task<(bool Success, List<Models.BinCollection> Collections)> GetBinCollectionDatesAsync(string uprn);
    }
}
