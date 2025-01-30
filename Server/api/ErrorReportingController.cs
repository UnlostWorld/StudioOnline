namespace StudioServer.Api;

using EmbedIO;
using EmbedIO.Routing;
using MongoDB.Bson;
using StudioServer.Client;
using System.Threading.Tasks;

public class ErrorReportingController : ApiController
{
	[Route(HttpVerbs.Post, "/error")]
	public async Task<string> Report()
	{
		ErrorReport data = await this.HttpContext.GetRequestDataAsync<ErrorReport>();

		var collection = this.Database.GetCollection("StudioServer-ErrorReporting", "Errors");

		ErrorReportEntry entry = new(data);
		entry.ShortCode = ShortCodeGenerator.Generate();
		await collection.InsertOneAsync(entry.ToBsonDocument());

		await Chat.ErrorReports.Report(data, entry.ShortCode);

		return entry.ShortCode;
	}

	public class ErrorReportEntry : Database.EntryBase
	{
		public ErrorReportEntry()
		{
		}

		public ErrorReportEntry(ErrorReport report)
		{
			this.Report = report;
		}

		public string? ShortCode { get; set; }
		public ErrorReport? Report { get; set; }
	}
}
