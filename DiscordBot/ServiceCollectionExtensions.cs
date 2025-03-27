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

using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

public static class ServiceCollectionExtensions
{
	public static void AddDiscordBot(this IServiceCollection self, Action<ServiceCollectionDiscordBotConfigurator>? configure = null)
	{
		self.AddOptions();
		self.AddSingleton<IDiscordBotService, DiscordBotService>();

		if (configure != null)
		{
			configure(new ServiceCollectionDiscordBotConfigurator(self));
		}
	}
}

public class ServiceCollectionDiscordBotConfigurator(IServiceCollection services)
{
	public void AddInteractionModule<T>()
		where T : IInteractionModuleBase
	{
		services.Configure<DiscordBotOptions>(options => options.InteractionModules.Add(typeof(T)));
	}
}

public class DiscordBotOptions
{
	public readonly List<Type> InteractionModules = new();
}