namespace StudioServer.Chat;

using Discord;
using StudioServer.Client;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public static class ErrorReports
{
	public static async Task Report(ErrorReport report, string shortCode)
	{
		ulong channelId = Program.Configuration.Current.BotErrorReportsChannel;

		if (channelId <= 0)
			return;

		IChannel channel = await Program.Bot.Client.GetChannelAsync(channelId);

		if (channel is ITextChannel textChannel)
		{
			List<FileAttachment> attachments = new List<FileAttachment>();

			if (report.LogFile != null)
			{
				MemoryStream mem = new MemoryStream();
				StreamWriter writer = new StreamWriter(mem);
				writer.Write(report.LogFile);

				attachments.Add(new FileAttachment(mem, "Log.txt"));
			}

			string message = $"❗\nError Report: **{shortCode}**\n{report.Message}\n\n{TimestampTag.FormatFromDateTime(System.DateTime.Now, TimestampTagStyles.LongDateTime)}";

			await textChannel.SendFilesAsync(attachments, message);
		}
	}
}
