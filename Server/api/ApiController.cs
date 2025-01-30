namespace StudioServer.Api;

using EmbedIO.WebApi;

public abstract class ApiController : WebApiController
{
	public Database Database => Program.Database;
}
