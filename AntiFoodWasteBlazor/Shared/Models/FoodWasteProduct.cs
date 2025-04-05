using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiFoodWasteBlazor.Shared.Models
{
    public class FoodWasteProduct
    {
        public string Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public string ImageUrl { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
    }
}
