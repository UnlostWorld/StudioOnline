namespace StudioServer;

using MongoDB.Bson;
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
	}

	public IMongoCollection<BsonDocument> GetCollection(string database, string collection)
	{
		if (this.client == null)
			throw new Exception("Database not connected");

		return this.client.GetDatabase(database).GetCollection<BsonDocument>(collection);
	}

	public class EntryBase
	{
		public ObjectId Id { get; set; }
	}
}