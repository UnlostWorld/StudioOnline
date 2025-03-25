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

namespace StudioOnline.Bot;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using StudioOnline.Chat;

public interface ISlashService
{
	Task RegisterModuleAsync<T>();
}

public class SlashService : ISlashService, IDisposable
{
	protected readonly ILogger<SlashService> Log;
	protected readonly IBotService Bot;
	private readonly Dictionary<string, (Type, MethodInfo)> commandHandlers = new();

	public SlashService(IBotService bot, ILogger<SlashService> logger)
	{
		this.Bot = bot;
		this.Log = logger;

		bot.RegisterSlashCommandCallback(this.OnSlashCommandExecuted);
		Task.Run(this.Start);
	}

	public async Task Start()
	{
		await this.RegisterModuleAsync<Commands>();
	}

	public void Dispose()
	{
	}

	public async Task RegisterModuleAsync<T>()
	{
		MethodInfo[] methods = typeof(T).GetMethods();
		foreach(MethodInfo method in methods)
		{
			SlashCommandAttribute? slashCommand = method.GetCustomAttribute<SlashCommandAttribute>();
			if (slashCommand == null)
				continue;

			this.commandHandlers.Add(slashCommand.Name, (typeof(T), method));

			SlashCommandBuilder globalCommand = new SlashCommandBuilder();
			globalCommand.WithName(slashCommand.Name);
			globalCommand.WithDescription(slashCommand.Description);
			await this.Bot.CreateGlobalApplicationCommandAsync(globalCommand.Build());
			this.Log.LogInformation($"Registered slash command: {slashCommand.Name}");
		}
	}

	public Task OnSlashCommandExecuted(SocketSlashCommand arg)
	{
		if (this.commandHandlers.TryGetValue(arg.CommandName, out (Type Type, MethodInfo Method) handler))
		{
			object? instance = Activator.CreateInstance(handler.Type);
			if (instance == null)
				return Task.CompletedTask;

			Task? task = handler.Method.Invoke(instance, [arg]) as Task;
			if (task != null)
			{
				Task.Run(async () =>
				{
					try
					{
						await task;
					}
					catch (Exception ex)
					{
						this.Log.LogError(ex, "Error in slash command handler");
					}
				});
			}
		}

		return Task.CompletedTask;
	}
}