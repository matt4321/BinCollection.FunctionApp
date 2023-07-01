namespace BinCollection.FunctionApp.Services
{
    public interface IBinCollectionService
    {
        Task RunAsync(bool onlySendIfTomorrow = false);
    }
}
