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
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;

public static class ServiceCollectionExtensions
{
	public static void AddStudioIdentity(this IServiceCollection self, Action<ServiceCollectionIdentityConfigurator> configure)
	{
		ServiceCollectionIdentityConfigurator config = new();
		configure.Invoke(config);

		string? dbConnectionString = config.ConnectionString;
		if (dbConnectionString == null)
			throw new("Missing connection string for MongoDb");

		IMongoDatabase openIddictDb = new MongoClient(dbConnectionString).GetDatabase("openiddict");
		OpenIddictBuilder openIddict = self.AddOpenIddict();
		openIddict.AddCore(options =>
		{
			options.UseMongoDb().UseDatabase(openIddictDb);
			options.UseQuartz();
		});

		IdentityBuilder ident = self.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(
			identity =>
			{
				identity.SignIn.RequireConfirmedAccount = true;
			},
			mongo =>
			{
				mongo.ConnectionString = dbConnectionString;
			});

		ident.AddDefaultUI();

		openIddict.AddClient(options =>
		{
			// Note: this sample only uses the authorization code flow,
			// but you can enable the other flows if necessary.
			options.AllowAuthorizationCodeFlow();

			// Register the signing and encryption credentials used to protect
			// sensitive data like the state tokens produced by OpenIddict.
			options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();

			// Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
			var aspnet = options.UseAspNetCore();
			aspnet.EnableRedirectionEndpointPassthrough();
			aspnet.DisableTransportSecurityRequirement();

			// Register the System.Net.Http integration.
			options.UseSystemNetHttp();

			// Register the Web providers integrations.
			//
			// Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
			// URI per provider, unless all the registered providers support returning a special "iss"
			// parameter containing their URL as part of authorization responses. For more information,
			// see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
			var providers = options.UseWebProviders();
			providers.AddDiscord(options =>
			{
				string? clientId = config.DiscordClientId;
				string? clientSecret = config.DiscordClientSecret;

				if (clientId == null || clientSecret == null)
					throw new Exception("No Discord client Id or Secret set.");

				options.SetClientId(clientId);
				options.SetClientSecret(clientSecret);
				options.SetRedirectUri("callback/login/discord");
			});
		});
	}
}

public static class IHostExtensions
{
	public static void UseStudioIdentity(this IHost self)
	{
		/*Task.Run(() =>
		{
			IMongoDatabase openIddictDb = new MongoClient(dbConnectionString).GetDatabase("openiddict");

			// If the openIddict db is empty, initialize its indexes.
			if (openIddictDb.ListCollectionNames().ToEnumerable().Count() <= 0)
				await OpenIddictFunctions.GenerateIndexes(self.Services);
		});*/
	}
}

public class ServiceCollectionIdentityConfigurator
{
	public string? ConnectionString;
	public string? DiscordClientId;
	public string? DiscordClientSecret;
}