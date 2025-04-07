using AntiFoodWasteBlazor.Shared.GitIgnoreFolder;
using AntiFoodWasteBlazor.Shared.Models;
using System.Reflection.Emit;
using System.Text.Json.Nodes;

namespace AntiFoodWasteBlazor.Client.Services
{
    public class SallingFoodWasteService
    {
		private readonly HttpClient _httpClient;

		public SallingFoodWasteService(HttpClient httpClient)
		{
			_httpClient = httpClient; // injiceret af Blazor
		}

        /// <summary>
        /// get foodwaste item based on zip code, for all products in all stores in zipcode
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
		public async Task<List<FoodWasteProduct>> GetFoodWasteProductsAsync(string? zipCode = null)
		{
			var result = new List<FoodWasteProduct>();

			var requestUri = $"api/foodwasteproxy/{zipCode}"; // kald til backend

			var response = await _httpClient.GetAsync(requestUri);
			if (!response.IsSuccessStatusCode)
				return result;

			var json = await response.Content.ReadAsStringAsync();
			var jsonArray = JsonNode.Parse(json)?.AsArray();
			if (jsonArray == null)
				return result;

			foreach (var entry in jsonArray)
			{
				var storeNode = entry["store"];
				var store = new Store
				{
                    Id = storeNode?["id"]?.ToString() ?? string.Empty,
                    Name = storeNode?["name"]?.ToString(),
					Street = storeNode?["address"]?["street"]?.ToString(),
					Zip = storeNode?["address"]?["zip"]?.ToString(),
					City = storeNode?["address"]?["city"]?.ToString()
				};

				var clearances = entry["clearances"]?.AsArray();
				if (clearances == null) continue;

				foreach (var item in clearances)
				{
					var product = new FoodWasteProduct
					{
						Description = item["product"]?["description"]?.ToString(),
                        Ean = item["product"]?["ean"]?.ToString(),
                        ImageUrl = item["product"]?["image"]?.ToString(),
						NewPrice = item["offer"]?["newPrice"]?.GetValue<decimal>() ?? 0,
						OriginalPrice = item["offer"]?["originalPrice"]?.GetValue<decimal>() ?? 0,
						DiscountPercent = item["offer"]?["percentDiscount"]?.GetValue<decimal>() ?? 0,
						EndDate = item["offer"]?["endTime"]?.GetValue<DateTime>() ?? DateTime.MinValue,
						StockCount = item["offer"]?["stock"]?.GetValue<decimal>() ?? 0,
						StockUnit = item["offer"]?["stockUnit"]?.ToString() ?? string.Empty,
						store = store
					};

					result.Add(product);
				}
			}

			return result;
		}

        /// <summary>
        /// Get food waste products based on store ID, for storebased list of products
        /// </summary>
        /// <param name="storeID"></param>
        /// <returns></returns>
        public async Task<List<FoodWasteProduct>> GetFoodWasteProductsBasedOnStoreIDAsync(string zipCode, string storeID)
        {
            var result = new List<FoodWasteProduct>();

			var requestUri = $"api/storeproxy?zipCode={zipCode}&storeID={storeID}";

			var response = await _httpClient.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
                return result;

            var json = await response.Content.ReadAsStringAsync();
            var jsonArray = JsonNode.Parse(json)?.AsArray();
            if (jsonArray == null)
                return result;

            foreach (var entry in jsonArray)
            {
                var storeNode = entry["store"];
                var store = new Store
                {
                    Id = storeNode?["id"]?.ToString() ?? string.Empty,
                    Name = storeNode?["name"]?.ToString(),
                    Street = storeNode?["address"]?["street"]?.ToString(),
                    Zip = storeNode?["address"]?["zip"]?.ToString(),
                    City = storeNode?["address"]?["city"]?.ToString()
                };

                var clearances = entry["clearances"]?.AsArray();
                if (clearances == null) continue;

                foreach (var item in clearances)
                {
                    var product = new FoodWasteProduct
                    {
                        Description = item["product"]?["description"]?.ToString(),
                        Ean = item["product"]?["ean"]?.ToString(),
                        ImageUrl = item["product"]?["image"]?.ToString(),
                        NewPrice = item["offer"]?["newPrice"]?.GetValue<decimal>() ?? 0,
                        OriginalPrice = item["offer"]?["originalPrice"]?.GetValue<decimal>() ?? 0,
                        DiscountPercent = item["offer"]?["percentDiscount"]?.GetValue<decimal>() ?? 0,
                        EndDate = item["offer"]?["endTime"]?.GetValue<DateTime>() ?? DateTime.MinValue,
                        LastUpdate = item["offer"]?["lastUpdate"]?.GetValue<DateTime>() ?? DateTime.MinValue,
						StockCount = item["offer"]?["stock"]?.GetValue<decimal>() ?? 0,
                        StockUnit = item["offer"]?["stockUnit"]?.ToString() ?? string.Empty,
                        store = store
                    };

                    result.Add(product);
                }
            }

            return result;
        }


    }
}
