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

namespace StudioOnline.Analytics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using StudioOnline;
using StudioOnline.DiscordBot;

public interface IAnalyticsService
{
	Task<string> Report(ErrorReport report);
}

public class AnalyticsService(IDiscordBotService bot)
	: IAnalyticsService
{
	public async Task<string> Report(ErrorReport report)
	{
		string shortCode = ShortCodeGenerator.Generate();

		List<FileAttachment> attachments = new List<FileAttachment>();
		if (!string.IsNullOrEmpty(report.LogFile))
		{
			MemoryStream mem = new MemoryStream();
			StreamWriter writer = new StreamWriter(mem);
			writer.Write(report.LogFile);

			attachments.Add(new FileAttachment(mem, "Log.txt"));
		}

		string message = $"❗\nError Report: **{shortCode}**\n{report.Message}\n\n{TimestampTag.FormatFromDateTime(DateTime.Now, TimestampTagStyles.LongDateTime)}";

		IEnumerable<DiscordBotGuildConfiguration> guildConfigs = await bot.GetGuildConfigurations();
		foreach(DiscordBotGuildConfiguration guildConfig in guildConfigs)
		{
			if (guildConfig.ErrorReportsChannel <= 0)
				continue;

			IGuild guild = bot.GetGuild(guildConfig.GuildId);
			IChannel channel = await guild.GetChannelAsync(guildConfig.ErrorReportsChannel);

			if (channel is ITextChannel textChannel)
			{
				await textChannel.SendFilesAsync(attachments, message);
			}
		}

		return shortCode;
	}
}