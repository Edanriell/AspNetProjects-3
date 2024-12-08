using System.Text.Json;
using Microsoft.Net.Http.Headers;

namespace ExchangeRateViewer;

public class ExchangeRatesClient
{
	private readonly HttpClient _httpClient;

	private readonly JsonSerializerOptions _serializerOptions = new()
																{
																	PropertyNamingPolicy = JsonNamingPolicy.CamelCase
																};

	public ExchangeRatesClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_httpClient.BaseAddress = new Uri("https://open.er-api.com/v6/");
		_httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "ExchangeRateViewer");
	}

	public async Task<ExchangeRates> GetLatestRatesAsync()
	{
		var result = await _httpClient.GetAsync("latest");
		result.EnsureSuccessStatusCode();

		var stream = await result.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<ExchangeRates>(stream, _serializerOptions);

		// or, using System.Net.Http.Json

		//return await _httpClient.GetFromJsonAsync<ExchangeRates>("latest", _serializerOptions);
	}
}