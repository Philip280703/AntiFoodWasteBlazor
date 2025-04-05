using AntiFoodWasteBlazor.Shared.GitIgnoreFolder;
using Microsoft.AspNetCore.Mvc;

namespace AntiFoodWasteBlazor.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class FoodWasteProxyController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string _apiKey;

		public FoodWasteProxyController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
			_apiKey = APIKey.GetApiKey(); // evt. træk fra config
		}

		[HttpGet("{zip}")]
		public async Task<IActionResult> GetFoodWaste(string zip)
		{
			var client = _httpClientFactory.CreateClient();
			var request = new HttpRequestMessage(
				HttpMethod.Get,
				$"https://api.sallinggroup.com/v1/food-waste/?zip={zip}");

			request.Headers.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

			var response = await client.SendAsync(request);
			var content = await response.Content.ReadAsStringAsync();

			return Content(content, "application/json");
		}
	}
}
