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

namespace StudioOnline.Utilities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class SubdomainRoute
{
	public static void RouteSubdomain(this IApplicationBuilder builder, string subdomain, string directory)
	{
		builder.Use(async (context, next) =>
		{
			if (context.Request.Host.Host.StartsWith(subdomain)
			&& context.Request.Path.Value?.EndsWith(".js") == false
			&& context.Request.Path.Value?.EndsWith(".css") == false)
			{
				context.Request.Host = new(context.Request.Host.Host.Substring(10), context.Request.Host.Port ?? 80);
				context.Request.Path = new PathString($"{directory}{context.Request.Path.Value}");
			}

			await next();
		});
	}
}
