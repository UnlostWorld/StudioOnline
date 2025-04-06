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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

[Route("plugins")]
[OutputCache(Tags = ["repository"])]
public class RepositoryController(IRepositoryService repositoryService)
	: Controller
{
	[HttpGet]
	public async Task<IActionResult> Get()
	{
		IEnumerable<RepositoryPlugin> plugins = await repositoryService.GetPlugins();
		JsonSerializerOptions op = new();
		op.WriteIndented = true;
		string json = JsonSerializer.Serialize(plugins, op);
		return this.Content(json);
	}
}

[Route("plugins/{pluginName}")]
[OutputCache(Tags = ["repository"], Duration = 60, NoStore = true)]
public class RepositoryDownloadController()
	: Controller
{
	[HttpGet]
	public IActionResult Get([FromRoute]string pluginName)
	{
		try
		{
			FileStream fs = System.IO.File.OpenRead($"{pluginName}-latest.zip");
			string contentType = "APPLICATION/octet-stream";
			string fileName = $"{pluginName}.zip";
			return this.File(fs, contentType, fileName);
		}
		catch (Exception)
		{
			return this.StatusCode(404);
		}
	}
}
