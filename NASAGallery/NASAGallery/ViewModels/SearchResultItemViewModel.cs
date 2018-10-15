using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NASAGallery.Repository;

namespace NASAGallery.ViewModels
{
    public class SearchResultItemViewModel
    {
        public SearchResultItemModel ItemModel { get; }

        public string Date => ItemModel.Data?.FirstOrDefault()?.DateCreated.ToString();

        public string Title => ItemModel.Data?.FirstOrDefault()?.Title;

        public string PreviewUrl
        {
            get
            {
                return ItemModel.Links?.FirstOrDefault(l => l.Rel == "preview" && l.MediaType == MediaType.Image)?.Href;
            }
        }

        public string ShortDescription
        {
            get
            {
                var descr = ItemModel.Data?.FirstOrDefault()?.Description;
                if(string.IsNullOrWhiteSpace(descr))
                    return string.Empty;
                
                return descr.Substring(0, Math.Min(descr.Length, 100)) + (descr.Length <= 100 ? "" : " ...");
            }
        }

        public SearchResultItemViewModel(SearchResultItemModel itemModel)
        {
            ItemModel = itemModel;
        }
    }
}
