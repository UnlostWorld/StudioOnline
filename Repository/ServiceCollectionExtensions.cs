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

namespace StudioOnline.Repository;

using Microsoft.Extensions.DependencyInjection;
using System;

public static class ServiceCollectionExtensions
{
	public static void AddPluginRepository(this IServiceCollection self, Action<ServiceCollectionRepositoryConfigurator>? configure = null)
	{
		self.AddOptions();
		self.AddSingleton<IRepositoryService, RepositoryService>();

		if (configure != null)
		{
			configure(new ServiceCollectionRepositoryConfigurator(self));
		}
	}
}

public class ServiceCollectionRepositoryConfigurator(IServiceCollection services)
{
	public void SetConnectionString(string? value) => services.Configure<RepositoryOptions>(options => options.ConnectionString = value);
}

public class RepositoryOptions
{
	public string? ConnectionString;
}