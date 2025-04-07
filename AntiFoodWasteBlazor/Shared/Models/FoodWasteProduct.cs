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
        public string Ean { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPercent { get; set; } 
		public string ImageUrl { get; set; }
        public DateTime EndDate { get; set; } 
        public decimal StockCount { get; set; }
        public string StockUnit { get; set; }
		public Store store { get; set; }    
    }
}
