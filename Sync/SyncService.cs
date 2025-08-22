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
	bool Status(string identifier, out IPAddress? address, out int? port);
}

public class SyncService : ISyncService
{
	protected readonly ILogger Log;
	private readonly Dictionary<string, SyncEntry> users = new();

	public SyncService(ILogger<SyncService> log)
	{
		this.Log = log;
		this.Log.LogInformation("Sync service online");
	}

	public void Update(string identifier, IPAddress address, int port)
	{
		if (!this.users.ContainsKey(identifier))
			this.users.Add(identifier, default);

		SyncEntry entry = this.users[identifier];
		entry.Address = address;
		entry.Port = port;
		this.users[identifier] = entry;
	}

	public bool Status(string identifier, out IPAddress? address, out int? port)
	{
		bool success = this.users.TryGetValue(identifier, out SyncEntry entry);
		address = entry.Address;
		port = entry.Port;
		return success;
	}

	public struct SyncEntry
	{
		public IPAddress Address;
		public int Port;
	}
}