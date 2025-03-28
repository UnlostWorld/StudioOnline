// .                    @@             _____ _______ _    _ _____ _____ ____
//          @       @@@@@             / ____|__   __| |  | |  __ \_   _/ __ \
//         @@@  @@@@                 | (___    | |  | |  | | |  | || || |  | |
//         @@@@@@@@@  @    @          \___ \   | |  | |  | | |  | || || |  | |
//        @@@@       @@@@@@@          ____) |  | |  | |__| | |__| || || |__| |
//    @@@@@             @@@          |_____/   |_|   \____/|_____/_____\____/
//     @@@      @@@@@@@  @@                   __                    ___
//      @@   @@@@    @@  @@                  /  \ |\ | |    | |\ | |__
//      @@  @@@     @@   @   @               \__/ | \| |___ | | \| |___
//    @@@@  @@@   @@@@   @@@@
//     @@@@  @@@@@  @@@@@@@         https://github.com/UnlostWorld/StudioOnline
//       @@@
//        @@@@@@@@@@@@@@                This software is licensed under the
//            @@@@  @                  GNU AFFERO GENERAL PUBLIC LICENSE v3

namespace StudioOnline.Api;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

public class AnalyticsController(IAnalyticsService analyticsService)
	: Controller
{
	[HttpPost]
	public async Task<IActionResult> Error([FromBody] ErrorReport report)
	{
		string code = await analyticsService.Report(report);
		return this.Content(code);
	}

	[HttpPost]
	public IActionResult Event([FromBody] AnalyticEvent report)
	{
		return this.StatusCode(200);
	}
}