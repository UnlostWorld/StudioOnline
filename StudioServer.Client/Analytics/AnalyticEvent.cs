namespace StudioServer.Client.Analytics;

using System.Text.Json;

public class AnalyticEvent
{
	public AnalyticEvents Event { get; set; }
	public string? EventData { get; set; }

	public Task Send()
	{
		string json = JsonSerializer.Serialize(this);
		return StudioServerApi.PostAsync("/Analytics/Event", json, "application/json");
	}

	public static void Send(AnalyticEvents evt, string? data = null)
	{
		AnalyticEvent eventObj = new();
		eventObj.Event = evt;
		eventObj.EventData = data;
		Task.Run(eventObj.Send);
	}
}

public enum AnalyticEvents
{
	None = 0,

	StudioStarted,
}