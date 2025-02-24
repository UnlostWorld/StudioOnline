namespace StudioServer.Client;

using System.Net;
using System.Text;

public static class StudioServerApi
{
	public static string Url = "http://unlostworld.duckdns.org/api/";

	private static readonly HttpClient client;

	static StudioServerApi()
	{
		HttpClientHandler handler = new();
		handler.AutomaticDecompression = DecompressionMethods.All;

		client = new(handler);
	}

	public static async Task<string> GetAsync(string uri)
	{
		using HttpResponseMessage response = await client.GetAsync(Url + uri);

		return await response.Content.ReadAsStringAsync();
	}

	public static async Task<string> PostAsync(string uri, string data, string contentType)
	{
		using HttpContent content = new StringContent(data, Encoding.UTF8, contentType);

		HttpRequestMessage requestMessage = new HttpRequestMessage()
		{
			Content = content,
			Method = HttpMethod.Post,
			RequestUri = new Uri(Url + uri)
		};

		using HttpResponseMessage response = await client.SendAsync(requestMessage);

		return await response.Content.ReadAsStringAsync();
	}
}
