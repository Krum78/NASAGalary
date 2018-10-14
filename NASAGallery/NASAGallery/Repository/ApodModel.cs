using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace NASAGallery.Repository
{
    public class ApodModel
    {
        public string Copyright { get; set; }

        public string Date { get; set; }

        public string Title { get; set; }

        public string Explanation { get; set; }

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

        public string HdUrl { get; set; }

        public string Url { get; set; }
    }
}
