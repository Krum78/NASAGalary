using Newtonsoft.Json;

namespace NASAGallery.Repository
{
    public class SearchResultMetaModel
    {
        [JsonProperty("total_hits")]
        public int TotalHits { get; set; }
    }
}
