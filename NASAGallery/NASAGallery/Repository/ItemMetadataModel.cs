using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace NASAGallery.Repository
{
    public class ItemMetadataModel
    {
        [JsonProperty("AVAIL:Copyright")]
        public string Copyright { get; set; }

        [JsonProperty("AVAIL:DateTaken")]
        public string Date { get; set; }

        [JsonProperty("AVAIL:Title")]
        public string Title { get; set; }

        [JsonProperty("AVAIL:Description")]
        public string Explanation { get; set; }

        [JsonProperty("AVAIL:MediaType")]
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

        [JsonProperty("Flickr:Url")]
        public string Url { get; set; }
    }
}
