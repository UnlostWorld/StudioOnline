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
using StudioOnline.DiscordBot;
using StudioOnline.Utilities;
using Microsoft.Extensions.Configuration;
using StudioOnline.Identity;
using StudioOnline.Analytics;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		var services = builder.Services;

		services.AddRazorPages();
		services.AddControllers();

		services.AddSingleton<IAnalyticsService, AnalyticsService>();

		services.AddDiscordBot(options =>
		{
			options.SetConnectionString(builder.Configuration.GetConnectionString("DiscordBot"));
			options.AddInteractionModule<EchoCommandModule>();
			options.AddInteractionModule<ErrorCommandModule>();
		});

		services.AddStudioIdentity(options =>
		{
			options.IdentityConnectionString = builder.Configuration.GetConnectionString("Identity");
			options.DiscordClientId = builder.Configuration["StudioOnline_DiscordClientId"];
			options.DiscordClientSecret = builder.Configuration["StudioOnline_DiscordClientSecret"];
		});

		WebApplication app = builder.Build();

		// Force HTTPS in production
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		////app.RouteSubdomain("marketplace", "/Marketplace");

		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();

		app.MapStaticAssets();
		app.MapRazorPages().WithStaticAssets();
		app.UseStaticFiles();

		app.MapControllerRoute("default", "api/{controller=Home}/{action=Index}");
		app.MapControllers();

		app.UseDiscordBot();
		app.UseStudioIdentity();

		app.Run();
	}
}