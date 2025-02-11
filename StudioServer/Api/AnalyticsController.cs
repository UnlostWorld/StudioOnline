namespace StudioServer.Api;

using Microsoft.AspNetCore.Mvc;
using StudioServer.Chat;
using StudioServer;
using StudioServer.Client.Analytics;

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