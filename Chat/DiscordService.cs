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

namespace StudioOnline.Chat;

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudioOnline.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface IDiscordService
{
	Task Report(ErrorReport report, string shortCode);
}

public class DiscordService : IDiscordService, IDisposable
{
	protected readonly ILogger<DiscordService> Log;
	protected readonly IConfiguration Configuration;
	protected readonly DiscordSocketClient Client;

	public DiscordService(ILogger<DiscordService> log, IConfiguration configuration)
	{
		this.Log = log;
		this.Configuration = configuration;
		this.Client = new();

		Task.Run(this.Start);
	}

	public async Task Start()
	{
		this.Client.Log += this.OnClientLog;
		this.Client.Ready += this.OnClientReady;
		this.Client.SlashCommandExecuted += this.OnClientSlashCommandExecuted;

		string? token = this.Configuration["StudioOnline:DiscordBotToken"];
		if (token == null)
		{
			this.Log.LogError("No Discord bot token set");
			return;
		}

		await this.Client.LoginAsync(TokenType.Bot, token);
		await this.Client.StartAsync();
		this.Log.LogInformation("Discord bot started");
	}

	public void Dispose()
	{
		this.Client.Dispose();
	}

	public async Task Report(ErrorReport report, string shortCode)
	{
		// TODO
		ulong channelId = 0;

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
			case LogSeverity.Critical: this.Log.LogError(arg.Exception, arg.Message); break;
			case LogSeverity.Error: this.Log.LogError(arg.Exception, arg.Message); break;
			case LogSeverity.Warning: this.Log.LogWarning(arg.Exception, arg.Message); break;
			case LogSeverity.Info: this.Log.LogInformation(arg.Exception, arg.Message); break;
			case LogSeverity.Verbose: this.Log.LogTrace(arg.Exception, arg.Message); break;
			case LogSeverity.Debug: this.Log.LogDebug(arg.Exception, arg.Message); break;
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

			this.Log.LogInformation("Registered commands");
		}
		catch (HttpException exception)
		{
			this.Log.LogError(exception, "Failed to register commands");
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
