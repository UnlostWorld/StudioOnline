namespace StudioServer.Api;

using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System.Threading.Tasks;

public class TestController : WebApiController
{
	[Route(HttpVerbs.Get, "/test")]
	public Task PostData()
	{
		Swan.Logging.Logger.Info("Hello World!");
		return Task.CompletedTask;
	}
}
