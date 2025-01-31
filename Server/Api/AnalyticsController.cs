namespace StudioServer.Api;

using EmbedIO;
using EmbedIO.Routing;
using MongoDB.Bson;
using MongoDB.Driver;
using StudioServer.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsController : ApiController
{
	[Route(HttpVerbs.Post, "/analytics")]
	public async Task Analytics()
	{
		AnalyticEvent data = await this.HttpContext.GetRequestDataAsync<AnalyticEvent>();

		var collection = this.Database.GetCollection("StudioServer-Analytics", "Events");

		AnalyticsEntry entry = new();
		entry.Event = data.Event;
		entry.Data = data.EventData;
		entry.Date = DateTime.UtcNow;
		await collection.InsertOneAsync(entry.ToBsonDocument());

		/*var filterBuilder = Builders<BsonDocument>.Filter;
		var filter = filterBuilder.Gt("Date", DateTime.UtcNow.AddHours(-1));
		List<BsonDocument> searchResult = collection.Find(filter).ToList();
		Logging.Shared.Information($">> {searchResult.Count} results");*/
	}

	public class AnalyticsEntry : Database.EntryBase
	{
		public AnalyticEvents Event { get; set; }
		public string? Data { get; set; }
		public DateTime? Date { get; set; }
	}
}