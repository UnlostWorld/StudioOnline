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

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;
using StudioOnline.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

[Route("Api/[controller]/[action]")]
public class IdentityController(
	SignInManager<ApplicationUser> signInManager,
	UserManager<ApplicationUser> userManager,
	ILogger<IdentityController> logger,
	IOutputCacheStore cache)
	: Controller
{
	[HttpPost]
	public IActionResult Connect(string provider)
	{
		// Request a redirect to the external login provider.
		AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, "/api/Identity/ConnectCallback");
		return new ChallengeResult(provider, properties);
	}

	[HttpGet]
	public async Task<IActionResult> ConnectCallback()
	{
		ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync();
		if (info == null)
			throw new Exception("Error loading external login information.");

		// Sign in the user with this external login provider if the user already has a login.
		SignInResult result = await signInManager.ExternalLoginSignInAsync(
			info.LoginProvider,
			info.ProviderKey,
			isPersistent: false,
			bypassTwoFactor: true);

		if (result.Succeeded)
		{
			logger.LogInformation($"{info.Principal?.Identity?.Name} logged in with {info.LoginProvider} provider.");
		}
		else if (result.IsLockedOut)
		{
			logger.LogWarning($"{info.Principal?.Identity?.Name} is locked out of {info.LoginProvider} provider.");
		}
		else
		{
			ApplicationUser user = new();
			user.UserName = info.Principal?.Identity?.Name;

			IdentityResult newUserResult = await userManager.CreateAsync(user);

			if (newUserResult.Succeeded)
			{
				newUserResult = await userManager.AddLoginAsync(user, info);
				if (newUserResult.Succeeded)
				{
					await signInManager.SignInAsync(user, isPersistent: false);
					logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
				}
			}
		}

		// flush the cache to update all pages with the user signed in.
		await cache.EvictByTagAsync("all", default);

		return new RedirectResult("/");
	}

	[HttpGet]
	public async Task<IActionResult> Disconnect()
	{
		await signInManager.SignOutAsync();

		// flush the cache to update all pages with the user signed out.
		await cache.EvictByTagAsync("all", default);

		return new RedirectResult("/");
	}
}