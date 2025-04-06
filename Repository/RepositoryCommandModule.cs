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

using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Microsoft.AspNetCore.OutputCaching;

#pragma warning disable

public class RepositoryCommandModule(IRepositoryService repository, IOutputCacheStore cache)
	: InteractionModuleBase<SocketInteractionContext>
{
	[CommandContextType(InteractionContextType.Guild)]
	[DefaultMemberPermissions(GuildPermission.Administrator)]
	[SlashCommand("update-plugin", "updates a plugin")]
	public async Task UpdatePlugin(string json)
	{
		RepositoryPlugin? plugin = JsonSerializer.Deserialize<RepositoryPlugin>(json);
		if (plugin == null)
		{
			await this.RespondAsync("Invalid plugin json", ephemeral: true);
			return;
		}

		await repository.AddOrUpdate(plugin);

		// flush the cache so that plugin list will update.
		await cache.EvictByTagAsync("repository", default);

		await this.RespondAsync("Done", ephemeral: true);
	}

	[CommandContextType(InteractionContextType.Guild)]
	[DefaultMemberPermissions(GuildPermission.Administrator)]
	[SlashCommand("upload-plugin", "uploads a new version of a plugin")]
	public async Task UploadPlugin(string name, IAttachment file)
	{
		await this.DeferAsync(true);
		await this.ModifyOriginalResponseAsync(properties => properties.Content = "Downloading");

		using HttpClient client = new();
		using HttpResponseMessage response = await client.GetAsync(file.Url);
		using FileStream fs = File.Create($"{name}-latest.zip");
		await response.Content.CopyToAsync(fs);

		// flush the cache so that plugin list will update.
		await cache.EvictByTagAsync("repository", default);

		await this.ModifyOriginalResponseAsync(properties => properties.Content = "Done");
	}
}