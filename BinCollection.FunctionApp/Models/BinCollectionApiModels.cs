using Newtonsoft.Json;

namespace BinCollection.FunctionApp.Models
{
    public class Data
    {
        public string __type { get; set; }

        [JsonProperty("ServiceHeaders")]
        public List<BinCollection> BinCollections { get; set; }
        public string ServiceName { get; set; }
    }

    public class BinCollectionResponse
    {
        [JsonProperty("d")]
        public List<Data> data { get; set; }
    }

    public class BinCollection
    {
        public string TaskType { get; set; }
        public DateTime Last { get; set; }
        public DateTime Next { get; set; }
        public string ScheduleDescription { get; set; }
    }

    public class BinCollectionRequest
    {
        public string uprn { get; set; }

        public string noticeBoard { get; set; }
    }
}
