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

namespace StudioOnline.Identity;

using System;
using System.Globalization;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;

public static class ServiceCollectionExtensions
{
	public static void AddStudioIdentity(this IServiceCollection self, Action<ServiceCollectionIdentityConfigurator> configure)
	{
		ServiceCollectionIdentityConfigurator config = new();
		configure.Invoke(config);

		AuthenticationBuilder authBuilder = self.AddAuthentication();
		if (config.DiscordClientId != null && config.DiscordClientSecret != null)
		{
			authBuilder.AddDiscord(options =>
			{
				options.ClientId = config.DiscordClientId;
				options.ClientSecret = config.DiscordClientSecret;

				// Get avatar
				options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
					string.Format(
						CultureInfo.InvariantCulture,
						"https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
						user.GetString("id"),
						user.GetString("avatar"),
						user.GetString("avatar")?.StartsWith("a_") == true ? "gif" : "png"));
			});
		}

		IdentityBuilder ident = self.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(
			identity =>
			{
				identity.SignIn.RequireConfirmedAccount = false;
				identity.SignIn.RequireConfirmedEmail = false;
				identity.SignIn.RequireConfirmedPhoneNumber = false;
				identity.Password.RequireNonAlphanumeric = false;
				identity.Password.RequiredLength = 4;
				identity.Password.RequiredUniqueChars = 0;
				identity.Password.RequireLowercase = false;
				identity.Password.RequireUppercase = false;
			},
			mongo =>
			{
				mongo.ConnectionString = config.IdentityConnectionString;
			});

		ident.AddDefaultUI();
	}
}

public static class IHostExtensions
{
	public static void UseStudioIdentity(this IHost self)
	{
	}
}

public class ServiceCollectionIdentityConfigurator
{
	public string? IdentityConnectionString;
	public string? DiscordClientId;
	public string? DiscordClientSecret;
}