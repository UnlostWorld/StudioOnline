namespace StudioServer;
using System;

public class Program
{
	public static readonly Database Database = new();
	public static readonly Server Server = new();

	public static void Main(string[] args)
	{
		Logging.Information("StudioServer starting");

		Database.Connect();
		Server.Start();

		Logging.Information("StudioServer started. Press 'Q' to terminate.");

		ConsoleKeyInfo info;
		do
		{
			info = Console.ReadKey(true);
		}
		while (info.Key != ConsoleKey.Q);

		Server.Dispose();
	}
}