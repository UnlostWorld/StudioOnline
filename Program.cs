namespace StudioServer;

using EmbedIO;
using EmbedIO.WebApi;
using StudioServer.Api;
using Serilog;
using System;

public class Program
{
	public static void Main(string[] args)
	{
		Swan.Logging.Logger.UnregisterLogger<Swan.Logging.ConsoleLogger>();
		Swan.Logging.Logger.RegisterLogger<LoggerBridge>();

		string url = "http://*:5202/";

		WebServerOptions options = new();
		options.WithUrlPrefix(url);
		options.WithMode(HttpListenerMode.EmbedIO);

		using WebServer server = new(options);
		server.StateChanged += (s, e) => Swan.Logging.Logger.Info($"WebServer New State - {e.NewState}");
		server.WithLocalSessionManager();

		server.WithWebApi("/api", m => m.WithController<TestController>());

		Logging.Information("Starting Server");
		server.RunAsync();

		Logging.Information("Server started. Press any key to terminate.");
		Console.ReadKey(true);
	}
}

public class LoggerBridge : Swan.Logging.ILogger
{
	private static readonly ILogger Logger;

	static LoggerBridge()
	{
		Logger = Logging.ForContext("Web");
	}

	public Swan.Logging.LogLevel LogLevel { get; }

	public void Dispose()
	{
	}

	public void Log(Swan.Logging.LogMessageReceivedEventArgs logEvent)
	{
		Serilog.Events.LogEventLevel level = ToSerilogLevel(logEvent.MessageType);
		Logger.Write(level, logEvent.Exception, logEvent.Message);
	}

	private static Serilog.Events.LogEventLevel ToSerilogLevel(Swan.Logging.LogLevel logLevel)
	{
		switch (logLevel)
		{
			case Swan.Logging.LogLevel.None: return Serilog.Events.LogEventLevel.Verbose;
			case Swan.Logging.LogLevel.Trace: return Serilog.Events.LogEventLevel.Verbose;
			case Swan.Logging.LogLevel.Debug: return Serilog.Events.LogEventLevel.Debug;
			case Swan.Logging.LogLevel.Info: return Serilog.Events.LogEventLevel.Information;
			case Swan.Logging.LogLevel.Warning: return Serilog.Events.LogEventLevel.Warning;
			case Swan.Logging.LogLevel.Error: return Serilog.Events.LogEventLevel.Error;
			case Swan.Logging.LogLevel.Fatal: return Serilog.Events.LogEventLevel.Fatal;
		}

		throw new NotSupportedException();
	}
}