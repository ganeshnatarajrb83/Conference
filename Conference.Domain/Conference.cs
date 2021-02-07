using Newtonsoft.Json;

namespace Conference.Domain
{
    public class Collection
    {
        public Items Items { get; set; }
    }

    public class Items
    {
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("data")]
        public Datum[] Data { get; set; }
        [JsonProperty("links")]
        public Link[] Links { get; set; }
    }

    public class Datum
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Link
    {
        [JsonProperty("rel")]
        public string Rel { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }
    }

}
