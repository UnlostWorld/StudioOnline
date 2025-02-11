namespace StudioServer.Chat;

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudioServer.Client.Analytics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface IDiscordService
{
	Task Report(ErrorReport report, string shortCode);
}

public class DiscordService(ILogger<DiscordService> log, IConfiguration configuration)
	: IDiscordService, IDisposable
{
	public readonly DiscordSocketClient Client = new();

	public async Task Start()
	{
		this.Client.Log += this.OnClientLog;
		this.Client.Ready += this.OnClientReady;
		this.Client.SlashCommandExecuted += this.OnClientSlashCommandExecuted;

		string? token = configuration["BotToken"];
		if (token == null)
		{
			log.LogError("No bot token set");
			return;
		}

		await this.Client.LoginAsync(TokenType.Bot, token);
		await this.Client.StartAsync();
	}

	public void Dispose()
	{
		this.Client.Dispose();
	}

	public async Task Report(ErrorReport report, string shortCode)
	{
		ulong channelId = ulong.Parse(configuration["BotErrorReportsChannel"] ?? "-1");

		if (channelId <= 0)
			return;

		IChannel channel = await this.Client.GetChannelAsync(channelId);

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

	private Task OnClientLog(global::Discord.LogMessage arg)
	{
		switch (arg.Severity)
		{
			case LogSeverity.Critical: log.LogError(arg.Exception, arg.Message); break;
			case LogSeverity.Error: log.LogError(arg.Exception, arg.Message); break;
			case LogSeverity.Warning: log.LogWarning(arg.Exception, arg.Message); break;
			case LogSeverity.Info: log.LogInformation(arg.Exception, arg.Message); break;
			case LogSeverity.Verbose: log.LogTrace(arg.Exception, arg.Message); break;
			case LogSeverity.Debug: log.LogDebug(arg.Exception, arg.Message); break;
		}

		return Task.CompletedTask;
	}

	private async Task OnClientReady()
	{
		try
		{
			SlashCommandBuilder globalCommand = new SlashCommandBuilder();
			globalCommand.WithName("error-logs-here");
			globalCommand.WithDescription("Sets the output chanel for error logs");
			globalCommand.WithDefaultMemberPermissions(GuildPermission.ManageChannels);
			await this.Client.CreateGlobalApplicationCommandAsync(globalCommand.Build());

			log.LogInformation("Registered commands");
		}
		catch (HttpException exception)
		{
			log.LogError(exception, "Failed to register commands");
		}
	}

	private async Task OnClientSlashCommandExecuted(SocketSlashCommand arg)
	{
		if (arg.CommandName == "error-logs-here")
		{
			if (arg.ChannelId == null)
				return;

			////this.Configuration.Current.BotErrorReportsChannel = (ulong)arg.ChannelId;
			////this.Configuration.Save();

			await arg.RespondAsync($"OK");
		}
	}
}
