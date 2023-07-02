namespace BinCollection.FunctionApp.Services
{
    public interface ITextService
    {
        Task SendText(string textMessage);
    }
}
