namespace StudioServer;

using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using System;

public class Database
{
	public ILogger Log;

	private MongoClient? client;

	public Database()
	{
		this.Log = Logging.ForContext<Database>();
	}

	public Configuration Configuration => Program.Configuration;

	public void Connect()
	{
		string? connectionString = this.Configuration.Current.DatabaseURI;
		if (connectionString == null)
		{
			this.Log.Error("No database URI set");
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