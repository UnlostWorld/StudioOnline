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

namespace StudioOnline.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public interface IRepositoryService
{
	Task<IEnumerable<RepositoryPlugin>> GetPlugins();
	Task AddOrUpdate(RepositoryPlugin plugin);
}

public class RepositoryService : IRepositoryService
{
	protected readonly IOptions<RepositoryOptions> Options;
	protected readonly IMongoCollection<RepositoryPlugin> Plugins;
	protected readonly ILogger<RepositoryService> Log;

	public RepositoryService(
		IOptions<RepositoryOptions> options,
		ILogger<RepositoryService> log)
	{
		this.Options = options;
		this.Log = log;

		MongoUrl mongoUrl = new MongoUrl(options.Value.ConnectionString);
		IMongoDatabase database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
		this.Plugins = database.GetCollection<RepositoryPlugin>("Plugins");
	}

	public async Task<IEnumerable<RepositoryPlugin>> GetPlugins()
	{
		IAsyncCursor<RepositoryPlugin> cursor = await this.Plugins.FindAsync(FilterDefinition<RepositoryPlugin>.Empty);
		return cursor.ToEnumerable();
	}

	public async Task AddOrUpdate(RepositoryPlugin plugin)
	{
		FilterDefinition<RepositoryPlugin> filter = Builders<RepositoryPlugin>.Filter.Eq(r => r.InternalName, plugin.InternalName);
		await this.Plugins.ReplaceOneAsync(filter, plugin);
	}
}

public class RepositoryPlugin
{
	public string? Author { get; set; }
	public string? Name { get; set; }
	public string? InternalName { get; set; }
	public string? Description { get; set; }
	public string? AssemblyVersion { get; set; }
	public string? RepoUrl { get; set; }
	public string? ApplicableVersion { get; set; }
	public int? DalamudApiLevel { get; set; }
	public List<string> Tags { get; set; } = new();
	public string? IconUrl { get; set; }
	public string? DownloadLinkInstall { get; set; }
	public string? DownloadLinkTesting { get; set; }
	public string? DownloadLinkUpdate { get; set; }
}