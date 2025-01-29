namespace StudioServer;
using System;

public class Program
{
	private static readonly Database Database = new();
	private static readonly Server Server = new();

	public static void Main(string[] args)
	{
		Logging.Information("StudioServer starting");

		Database.Connect();
		Server.Start();

		Logging.Information("StudioServer started. Press any key to terminate.");
		Console.ReadKey(true);
	}
}