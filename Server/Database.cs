namespace StudioServer;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;

public class Database
{
	private MongoClient? client;

	public void Connect()
	{
		var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
		if (connectionString == null)
		{
			Console.WriteLine("You must set your 'MONGODB_URI' environment variable");
			return;
		}

		this.client = new MongoClient(connectionString);

		IMongoCollection<BsonDocument> col = this.client.GetDatabase("StudioAnalytics").GetCollection<BsonDocument>("Analytics");

		Hit hit = new();

		col.InsertOne(hit.ToBsonDocument());

		Logging.Information($"{col}");
	}
}

public class Hit
{
	public ObjectId Id { get; set; }

	[BsonElement("date")]
	public DateTime Date { get; set; } = DateTime.UtcNow;
}