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

namespace StudioOnline;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using StudioOnline.DiscordBot;
using StudioOnline.Utilities;
using StudioOnline.Data;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		var services = builder.Services;

		services.AddRazorPages();

		services.AddControllers();
		services.AddDiscordBot(options =>
		{
			options.AddInteractionModule<EchoCommandModule>();
		});

		// OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
		// (like pruning orphaned authorizations/tokens from the database) at regular intervals.
		/*services.AddQuartz(options =>
		{
			options.UseSimpleTypeLoader();
			options.UseInMemoryStore();
		});

		// Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
		services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);*/

		services.AddDbContext<IddictContext>(options =>
		{
			options.UseInMemoryDatabase("Iddict");
			options.UseOpenIddict();
		});

		services.AddDbContext<FilesContext>(options =>
		{
			options.UseInMemoryDatabase("Files");
		});

		var ident = services.AddDefaultIdentity<IdentityUser>(options =>
		{
			options.SignIn.RequireConfirmedAccount = true;
		});

		ident.AddEntityFrameworkStores<IddictContext>();

		var openIddict = services.AddOpenIddict();
		openIddict.AddCore(options =>
		{
			options.UseEntityFrameworkCore().UseDbContext<IddictContext>();
			options.UseQuartz();
		});

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
				string? clientId = builder.Configuration["StudioOnline_DiscordClientId"];
				string? clientSecret = builder.Configuration["StudioOnline_DiscordClientSecret"];

				if (clientId == null || clientSecret == null)
					throw new Exception("No Discord client Id or Secret set.");

				options.SetClientId(clientId);
				options.SetClientSecret(clientSecret);
				options.SetRedirectUri("callback/login/discord");
			});
		});

		WebApplication app = builder.Build();

		// Force HTTPS in production
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.RouteSubdomain("marketplace", "/Marketplace");

		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();

		app.MapStaticAssets();
		app.MapRazorPages().WithStaticAssets();

		app.MapControllerRoute("default", "api/{controller=Home}/{action=Index}");
		app.MapControllers();

		app.UseDiscordBot();

		app.Run();
	}
}