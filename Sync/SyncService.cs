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

using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;

public interface ISyncService
{
	void Update(string identifier, IPAddress address, int port);
}

public class SyncService : ISyncService
{
	protected readonly ILogger Log;
	private readonly Dictionary<string, (IPAddress, int)> users = new();

	public SyncService(ILogger<SyncService> log)
	{
		this.Log = log;
		this.Log.LogInformation("Sync service online");
	}

	public void Update(string identifier, IPAddress address, int port)
	{
		this.users[identifier] = (address, port);
	}
}