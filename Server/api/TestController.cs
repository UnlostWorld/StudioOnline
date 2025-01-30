namespace StudioServer.Api;

using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System.IO;
using System.Threading.Tasks;

public class TestController : WebApiController
{
	[Route(HttpVerbs.Get, "/test")]
	public Task Test()
	{
		using StreamWriter writer = new(this.Response.OutputStream);
		writer.WriteLine("Hello from the unlost world");
		return Task.CompletedTask;
	}
}
