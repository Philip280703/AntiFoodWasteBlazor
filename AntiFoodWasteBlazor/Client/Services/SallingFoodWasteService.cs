using AntiFoodWasteBlazor.Shared.GitIgnoreFolder;
using AntiFoodWasteBlazor.Shared.Models;
using System.Reflection.Emit;
using System.Text.Json.Nodes;

namespace AntiFoodWasteBlazor.Client.Services
{
    public class SallingFoodWasteService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey;
        private string requestUri;

        public SallingFoodWasteService()
        {
            _apiKey = APIKey.GetApiKey();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.sallinggroup.com/v1/food-waste/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            
        }

        public async Task<List<FoodWasteProduct>> GetFoodWasteProductsAsync(string? zipCode = null)
        {
            var result = new List<FoodWasteProduct>();

			if (!string.IsNullOrEmpty(zipCode))
			{
				requestUri += $"?zip={zipCode}";
			}

			var response = await _httpClient.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                return result;
            }

            var json = await response.Content.ReadAsStringAsync();
            var jsonArray = JsonNode.Parse(json)?.AsArray();
            if (jsonArray == null)
            {
                return result;
            }

            foreach (var entry in jsonArray)
            { 
                var storeNode = entry["store"];
                var store = new Store
                {
                    Name = storeNode?["name"]?.ToString(),
                    Street = storeNode?["address"]["street"]?.ToString(),
                    Zip = storeNode?["address"]["zip"]?.ToString(),
                    City = storeNode?["address"]["city"]?.ToString()
                };

                var clearances = entry["clearances"]?.AsArray();
                if (clearances == null) continue;

                foreach (var item in clearances) 
                {
                    var product = new FoodWasteProduct
                    {
                        Description = item["product"]?["description"]?.ToString(),
                        ImageUrl = item["product"]?["imageUrl"]?.ToString(),
                        NewPrice = item["offer"]?["newPrice"]?.GetValue<decimal>() ?? 0,
                        OriginalPrice = item["offer"]?["originalPrice"]?.GetValue<decimal>() ?? 0,
                        store = store
                    };
                    result.Add(product);

                }
            }

            return result;

        }


    }
}
