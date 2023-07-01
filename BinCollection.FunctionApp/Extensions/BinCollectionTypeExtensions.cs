namespace BinCollection.FunctionApp.Helpers
{
    public static class BinCollectionTypeExtensions
    {
        public static string ToBinCollectionText(this string binCollectionTaskName)
        {
            switch (binCollectionTaskName)
            {
                case "Collect Communal Food":
                    {
                        return "Food";
                    }
                case "Collect Communal Recycling":
                    {
                        return "Recycling";
                    }
                case "Collect Communal Refuse":
                    {
                        return "General Waste";
                    }
                default:
                    throw new Exception("Unsupported mapping");
            }
        }
    }
}
