using System;
using System.Collections.Generic;
using System.Linq;

namespace WebRole2.Models
{
    public class ImageProduct
    {
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public ImageryType ImageType { get; set; }
        public string AndroidProductId { get; set; }
        public string IOSProductId { get; set; }

    }

    public enum ImageryType
    {
        BALLOON,
        BALLOON_BACKGROUND
    }
}