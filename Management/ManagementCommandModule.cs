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

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;

public class ManagementCommandModule(IHostApplicationLifetime lifetime)
	: InteractionModuleBase<SocketInteractionContext>
{
	public enum Operations
	{
		Update,
		Shutdown,
	}

	[DefaultMemberPermissions(GuildPermission.Administrator)]
	[SlashCommand("manage", "Manage the webserver process")]
	public async Task OnlineUpdate(Operations operation)
	{
		await this.RespondAsync($"{operation} in progress.", ephemeral: true);

		try
		{
			Task t = this.Run(operation);
		}
		catch (Exception ex)
		{
			await this.ModifyOriginalResponseAsync(op =>
			{
				op.Content = $"Error: {ex.Message}";
			});
		}
	}

	private Task Run(Operations operation)
	{
		switch (operation)
		{
			case Operations.Update: return this.Update();
			case Operations.Shutdown: return this.Shutdown();
		}

		throw new NotImplementedException();
	}

	private Task Update()
	{
		ProcessStartInfo startInfo = new();
		startInfo.FileName = "bash";
		startInfo.Arguments = $"Scripts/Update.sh";
		startInfo.UseShellExecute = true;

		Process process = new();
		process.StartInfo = startInfo;
		process.Start();

		////lifetime.StopApplication();

		return Task.CompletedTask;
	}

	private Task Shutdown()
	{
		lifetime.StopApplication();
		return Task.CompletedTask;
	}
}