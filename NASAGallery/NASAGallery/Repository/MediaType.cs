using System;
using System.Collections.Generic;
using System.Text;

namespace NASAGallery.Repository
{
    [Flags]
    public enum MediaType
    {
        None = 0,
        Image = 1,
        Video = 2,
        Audio = 4
    }
}
