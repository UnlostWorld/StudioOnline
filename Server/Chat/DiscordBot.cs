namespace StudioServer.Chat;

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

public class DiscordBot : IDisposable
{
	public readonly DiscordSocketClient Client;

	public DiscordBot()
	{
		this.Log = Logging.ForContext<DiscordBot>();
		this.Client = new();
	}

	public Database Database => Program.Database;
	public Configuration Configuration => Program.Configuration;
	protected ILogger Log { get; private set; }

	public async Task Start()
	{
		this.Client.Log += this.OnClientLog;
		this.Client.Ready += this.OnClientReady;
		this.Client.SlashCommandExecuted += this.OnClientSlashCommandExecuted;

		string? token = this.Configuration.Current.BotToken;
		if (token == null)
		{
			this.Log.Error("No bot token set");
			return;
		}

		await this.Client.LoginAsync(TokenType.Bot, token);
		await this.Client.StartAsync();
	}

	public void Dispose()
	{
		this.Client.Dispose();
	}

	private static LogEventLevel ToSerilog(LogSeverity severity)
	{
		switch (severity)
		{
			case LogSeverity.Critical: return LogEventLevel.Fatal;
			case LogSeverity.Error: return LogEventLevel.Error;
			case LogSeverity.Warning: return LogEventLevel.Warning;
			case LogSeverity.Info: return LogEventLevel.Information;
			case LogSeverity.Verbose: return LogEventLevel.Verbose;
			case LogSeverity.Debug: return LogEventLevel.Debug;
		}

		throw new NotSupportedException();
	}

	private Task OnClientLog(global::Discord.LogMessage arg)
	{
		this.Log.Write(ToSerilog(arg.Severity), arg.Exception, arg.Message);
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

			this.Log.Information("Registered commands");
		}
		catch (HttpException exception)
		{
			this.Log.Error(exception, "Failed to register commands");
		}
	}

	private async Task OnClientSlashCommandExecuted(SocketSlashCommand arg)
	{
		if (arg.CommandName == "error-logs-here")
		{
			if (arg.ChannelId == null)
				return;

			this.Configuration.Current.BotErrorReportsChannel = (ulong)arg.ChannelId;
			this.Configuration.Save();

			await arg.RespondAsync($"OK");
		}
	}
}
