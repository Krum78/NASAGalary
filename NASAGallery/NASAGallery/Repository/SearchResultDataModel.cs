using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NASAGallery.Repository
{
    public class SearchResultDataModel
    {
        public string Center { get; set; }

        [JsonProperty("date_created")]
        public DateTime DateCreated { get; set; }

        public string Description { get; set; }

        public List<string> Keywords { get; set; }

        [JsonProperty("media_type")]
        public string MediaTypeString { get; set; }

        public MediaType? MediaType
        {
            get
            {
                if (Enum.TryParse(MediaTypeString, true, out MediaType value))
                {
                    return value;
                }

                return null;
            }
        }

        [JsonProperty("nasa_id")]
        public string NasaId { get; set; }

        public string Title { get; set; }


    }
}
