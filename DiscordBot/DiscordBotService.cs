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

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudioOnline.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface IDiscordBotService
{
	Task Report(ErrorReport report, string shortCode);
	void Start();
}

public class DiscordBotService : IDiscordBotService, IDisposable
{
	protected readonly IOptions<DiscordBotOptions> Options;
	protected readonly ILogger<DiscordBotService> Log;
	protected readonly IConfiguration Configuration;
	protected readonly IServiceProvider Services;
	protected readonly DiscordSocketClient Client;
	protected readonly InteractionService Interactions;

	public DiscordBotService(IServiceProvider services, ILogger<DiscordBotService> log, IConfiguration configuration, IOptions<DiscordBotOptions> options)
	{
		this.Services = services;
		this.Log = log;
		this.Configuration = configuration;
		this.Options = options;

		this.Client = new();
		this.Interactions = new(this.Client);
	}

	public void Dispose()
	{
		this.Client.Dispose();
	}

	public void Start()
	{
		Task.Run(this.StartAsync);
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

			string message = $"❗\nError Report: **{shortCode}**\n{report.Message}\n\n{TimestampTag.FormatFromDateTime(DateTime.Now, TimestampTagStyles.LongDateTime)}";

			await textChannel.SendFilesAsync(attachments, message);
		}
	}

	private async Task StartAsync()
	{
		try
		{
			this.Client.Log += this.OnClientLog;
			this.Client.InteractionCreated += this.OnClientInteractionCreated;
			this.Client.Ready += this.OnClientReady;

			string? token = this.Configuration["StudioOnline_DiscordBotToken"];
			if (token == null)
			{
				this.Log.LogError("No Discord bot token set");
				return;
			}

			await this.Client.LoginAsync(TokenType.Bot, token);
			await this.Client.StartAsync();

			this.Log.LogInformation("Discord bot started");
		}
		catch (Exception ex)
		{
			this.Log.LogError(ex, "Error starting Discord Bot");
		}
	}

	private async Task OnClientReady()
	{
		foreach (Type moduleType in this.Options.Value.InteractionModules)
			await this.Interactions.AddModuleAsync(moduleType, this.Services);

		await this.Interactions.RegisterCommandsGloballyAsync();
	}

	private Task OnClientInteractionCreated(SocketInteraction interaction)
	{
		SocketInteractionContext context = new(this.Client, interaction);
		return this.Interactions.ExecuteCommandAsync(context, this.Services);
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
}