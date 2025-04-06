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

namespace StudioOnline.DiscordBot;

using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Microsoft.AspNetCore.OutputCaching;
using StudioOnline.Repository;

public class PluginsCommandModule(IRepositoryService repository, IOutputCacheStore cache)
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
}