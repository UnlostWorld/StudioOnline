namespace StudioServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudioServer.Chat;
using StudioServer.Utilities;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		builder.Services.AddRazorPages();

		builder.Services.AddControllers();
		builder.Services.AddScoped<IDiscordService, DiscordService>();

		WebApplication app = builder.Build();

		// Force HTTPS in production
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.RouteSubdomain("marketplace", "/Marketplace");

		app.UseRouting();
		app.UseAuthorization();

		app.MapStaticAssets();
		app.MapRazorPages().WithStaticAssets();

		app.MapControllerRoute("default", "api/{controller=Home}/{action=Index}");
		app.MapControllers();

		app.Run();
	}
}