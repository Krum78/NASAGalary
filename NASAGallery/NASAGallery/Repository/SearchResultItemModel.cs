using System.Collections.Generic;

namespace NASAGallery.Repository
{
    public class SearchResultItemModel
    {
        public List<LinkModel> Links { get; set; }
        public List<SearchResultDataModel> Data { get; set; }
        public string Href { get; set; }
    }
}
