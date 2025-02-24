namespace StudioServer.Client.Analytics;

using System.Threading.Tasks;
using System.Text.Json;

public class ErrorReport
{
	public string? Message { get; set; }
	public string? LogFile { get; set; }

	public Task<string> Send()
	{
		string json = JsonSerializer.Serialize(this);
		return StudioServerApi.PostAsync("/Analytics/Error", json, "application/json");
	}
}
