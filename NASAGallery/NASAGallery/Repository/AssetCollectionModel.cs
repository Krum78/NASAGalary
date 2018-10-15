using System;
using System.Collections.Generic;
using System.Text;

namespace NASAGallery.Repository
{
    public class AssetCollectionModel
    {
        public string Href { get; set; }

        public List<ItemModel> Items { get; set; }
    }
}
