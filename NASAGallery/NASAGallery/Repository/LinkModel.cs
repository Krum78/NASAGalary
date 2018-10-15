using System;
using System.Collections.Generic;
using System.Text;

namespace NASAGallery.Repository
{
    public class LinkModel
    {
        public string Rel { get; set; }

        public string Prompt { get; set; }

        public string Href { get; set; }

        public string Render { get; set; }

        public MediaType MediaType
        {
            get
            {
                if (Enum.TryParse(Render, true, out MediaType value))
                {
                    return value;
                }

                return MediaType.None;
            }
        }
    }
}
