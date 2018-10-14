using System.Collections.Generic;

namespace NASAGallery.Repository
{
    public class SearchResultCollectionModel
    {
        public SearchResultMetaModel Metadata { get; set; }

        public string Href { get; set; }

        public List<LinkModel> Links { get; set; }

        public List<SearchResultItemModel> Items { get; set; }

        public string Version { get; set; }
    }
}
