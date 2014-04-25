using System;
using WebRole2.Models;

namespace POPpicLibrary
{
	public class GameImageryItemViewModel
	{
		public string ItemName {get { return this.product.ItemName; } }
		public string ItemDescription {get { return this.product.ItemDescription; } }
		public string ImageUrl{get { return this.product.ImageUrl; } }
		public string ThumbnailUrl {get { return this.product.ThumbnailUrl; } }
		public ImageryType ImageType {get { return this.product.ImageType; } }
		public string AndroidProductId {get { return this.product.AndroidProductId; } }
		public string IOSProductId {get { return this.product.IOSProductId; } }

		public bool IsLocked { get; set;}

//		public static GameImageryItemViewModel Create(string name, string description, int resourceId, ImageryType type ) {
//			var item = new GameImageryItemViewModel ();
//			item.ImageType = type;
//			item.ItemName = name;
//			item.ItemDescription = description;
//			item.ResourceId = resourceId;
//			return item;
//		}

		public ImageProduct product { get; private set; }
		public GameImageryItemViewModel(ImageProduct product)
		{
			this.product = product;
		}
	}
}

