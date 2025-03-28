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

using System;
using System.Threading.Tasks;
using Discord.Interactions;
using StudioOnline.Api;

public class ErrorCommandModule(IDiscordBotService bot)
	: InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("error-reports-here", "Set the output channel for error reports")]
	public async Task ErrorReportsHere()
	{
		var config = await bot.GetGuildConfiguration(this.Context.Guild.Id);
		config.ErrorReportsChannel = this.Context.Channel.Id;
		await bot.SetGuildConfiguration(config);

		await this.RespondAsync("Done", ephemeral: true);
	}

	[SlashCommand("generate-test-error", "Generate a test error for the error reporting system")]
	public async Task GenerateTestError()
	{
		ErrorReport report = new();

		try
		{
			throw new Exception("A Test Error, generated server-side.");
		}
		catch (Exception ex)
		{
			report.Message = ex.Message;
			report.LogFile = "A Test error log file. \n\n" + ex.StackTrace;
		}

		string shortCode = ShortCodeGenerator.Generate();
		await bot.Report(report, shortCode);
		await this.RespondAsync("Done", ephemeral: true);
	}
}