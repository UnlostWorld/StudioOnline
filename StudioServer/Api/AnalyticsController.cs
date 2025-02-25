namespace StudioServer.Api;

using Microsoft.AspNetCore.Mvc;
using StudioServer.Chat;
using StudioServer;

public enum AnalyticEvents
{
	None = 0,

	StudioStarted,
}

public class AnalyticEvent
{
	public AnalyticEvents Event { get; set; }
	public string? EventData { get; set; }
}

public class ErrorReport
{
	public string? Message { get; set; }
	public string? LogFile { get; set; }
}

public class AnalyticsController(IDiscordService discord)
	: Controller
{
	[HttpPost]
	public IActionResult Error([FromBody] ErrorReport report)
	{
		string shortcode = ShortCodeGenerator.Generate();

		discord.Report(report, shortcode);

		return this.Content(shortcode);
	}

	[HttpPost]
	public IActionResult Event([FromBody] AnalyticEvent report)
	{
		return this.StatusCode(200);
	}
}