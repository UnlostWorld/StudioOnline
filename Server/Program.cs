namespace StudioServer;

using StudioServer.Chat;
using StudioServer.Client;
using System;
using System.Threading.Tasks;

public class Program
{
	public static readonly Configuration Configuration = new();
	public static readonly Database Database = new();
	public static readonly Server Server = new();
	public static readonly DiscordBot Bot = new();

	public static void Main(string[] args)
	{
		Task.Run(async () =>
		{
			Configuration.Load();
			Database.Connect();
			Server.Start();
			await Bot.Start();
		});

		Logging.Information("Press 'Q' to terminate.");

		ConsoleKeyInfo info;
		do
		{
			info = Console.ReadKey(true);
		}
		while (info.Key != ConsoleKey.Q);

		Server.Dispose();
		Bot.Dispose();
	}
}