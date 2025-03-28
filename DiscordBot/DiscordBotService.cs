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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using StudioOnline.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface IDiscordBotService
{
	Task Report(ErrorReport report, string shortCode);
	void Start();

	Task<DiscordBotGuildConfiguration> GetGuildConfiguration(ulong guildId);
	Task SetGuildConfiguration(DiscordBotGuildConfiguration config);
}

public class DiscordBotGuildConfiguration
{
	[BsonElement("_id")]
	public ObjectId ObjectId { get; set; }

	public ulong GuildId { get; set; }
	public ulong ErrorReportsChannel { get; set; }
}

public class DiscordBotService : IDiscordBotService, IDisposable
{
	protected readonly IOptions<DiscordBotOptions> Options;
	protected readonly ILogger<DiscordBotService> Log;
	protected readonly IConfiguration Configuration;
	protected readonly IServiceProvider Services;
	protected readonly DiscordSocketClient Client;
	protected readonly InteractionService Interactions;
	protected readonly IMongoCollection<DiscordBotGuildConfiguration> GuildConfigurations;

	public DiscordBotService(
		IServiceProvider services,
		ILogger<DiscordBotService> log,
		IConfiguration configuration,
		IOptions<DiscordBotOptions> options)
	{
		this.Services = services;
		this.Log = log;
		this.Configuration = configuration;
		this.Options = options;

		this.Client = new();
		this.Interactions = new(this.Client);

		MongoUrl mongoUrl = new MongoUrl(options.Value.ConnectionString);
		IMongoDatabase database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
		this.GuildConfigurations = database.GetCollection<DiscordBotGuildConfiguration>("Guilds");
	}

	public void Dispose()
	{
		this.Client.Dispose();
	}

	public void Start()
	{
		Task.Run(this.StartAsync);
	}

	public async Task<DiscordBotGuildConfiguration> GetGuildConfiguration(ulong guildId)
	{
		FilterDefinition<DiscordBotGuildConfiguration> filter = Builders<DiscordBotGuildConfiguration>.Filter.Eq(r => r.GuildId, guildId);
		DiscordBotGuildConfiguration? configuration = await this.GuildConfigurations.Find(filter).FirstOrDefaultAsync();

		if (configuration == null)
		{
			configuration = new();
			configuration.GuildId = guildId;
			await this.GuildConfigurations.InsertOneAsync(configuration);
		}

		return configuration;
	}

	public async Task SetGuildConfiguration(DiscordBotGuildConfiguration configuration)
	{
		FilterDefinition<DiscordBotGuildConfiguration> filter = Builders<DiscordBotGuildConfiguration>.Filter.Eq(r => r.GuildId, configuration.GuildId);
		await this.GuildConfigurations.ReplaceOneAsync(filter, configuration);
	}

	public async Task Report(ErrorReport report, string shortCode)
	{
		List<FileAttachment> attachments = new List<FileAttachment>();
		if (!string.IsNullOrEmpty(report.LogFile))
		{
			MemoryStream mem = new MemoryStream();
			StreamWriter writer = new StreamWriter(mem);
			writer.Write(report.LogFile);

			attachments.Add(new FileAttachment(mem, "Log.txt"));
		}

		string message = $"❗\nError Report: **{shortCode}**\n{report.Message}\n\n{TimestampTag.FormatFromDateTime(DateTime.Now, TimestampTagStyles.LongDateTime)}";

		IEnumerable<DiscordBotGuildConfiguration> guildConfigs = this.GuildConfigurations.Find(FilterDefinition<DiscordBotGuildConfiguration>.Empty).ToEnumerable();
		foreach(DiscordBotGuildConfiguration guildConfig in guildConfigs)
		{
			if (guildConfig.ErrorReportsChannel <= 0)
				continue;

			SocketGuild guild = this.Client.GetGuild(guildConfig.GuildId);
			SocketGuildChannel channel = guild.GetChannel(guildConfig.ErrorReportsChannel);

			if (channel is ITextChannel textChannel)
			{
				await textChannel.SendFilesAsync(attachments, message);
			}
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