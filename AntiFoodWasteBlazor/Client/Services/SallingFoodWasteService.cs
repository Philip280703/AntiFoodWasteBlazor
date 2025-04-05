﻿using AntiFoodWasteBlazor.Shared.GitIgnoreFolder;
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

		public async Task<List<FoodWasteProduct>> GetFoodWasteProductsAsync(string? zipCode = null)
		{
			var result = new List<FoodWasteProduct>();

			var requestUri = $"api/foodwasteproxy/{zipCode}"; // kald til din backend

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
						ImageUrl = item["product"]?["image"]?.ToString(),
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
