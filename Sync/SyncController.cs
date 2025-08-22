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

namespace StudioOnline.Sync;

using System.Net;
using Microsoft.AspNetCore.Mvc;

public class SyncHeartbeat
{
	public string? Identifier { get; set; }
	public int? Port { get; set; }
}

[Route("Api/[controller]/[action]")]
public class SyncController(ISyncService syncService)
	: Controller
{
	[HttpPost]
	public IActionResult Heartbeat([FromBody] SyncHeartbeat heartbeat)
	{
		string? identifier = heartbeat.Identifier;
		IPAddress? ip = this.HttpContext.Connection.RemoteIpAddress;
		int? port = heartbeat.Port;

		if (identifier == null || ip == null || port == null)
			return this.BadRequest();

		syncService.Update(identifier, ip, port.Value);

		return this.Ok();
	}
}