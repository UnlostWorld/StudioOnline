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

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using StudioOnline.DiscordBot;
using StudioOnline.Utilities;
using Microsoft.Extensions.Configuration;
using StudioOnline.Identity;

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

		services.AddStudioIdentity(options =>
		{
			options.OpenIddictConnectionString = builder.Configuration.GetConnectionString("OpenIddict");
			options.IdentityConnectionString = builder.Configuration.GetConnectionString("Identity");
			options.DiscordClientId = builder.Configuration["StudioOnline_DiscordClientId"];
			options.DiscordClientSecret = builder.Configuration["StudioOnline_DiscordClientSecret"];
		});

		services.AddQuartz(options =>
		{
			options.UseSimpleTypeLoader();
			options.UseInMemoryStore();
		});

		// Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
		services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

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
		app.UseStudioIdentity();

		app.Run();
	}
}