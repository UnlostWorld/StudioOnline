namespace StudioServer;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Diagnostics;
using System.IO;

public static class Logging
{
	public static readonly LoggerConfiguration Configuration;
	private static readonly ILogger Logger;

	static Logging()
	{
		Formatter formatter = new();

		Configuration = new LoggerConfiguration();
		Configuration.WriteTo.Debug(formatter);
		Configuration.WriteTo.Console(formatter);

		Logger = Configuration.CreateLogger();
	}

	public static ILogger Shared => Logger;

	public static ILogger ForContext<T>() => ForContext(typeof(T));
	public static ILogger ForContext(Type type) => ForContext(type.Name);

	public static ILogger ForContext(string context)
	{
		return Logger.ForContext("Context", context);
	}

	public static void Information(string message) => Shared.Information(message);
}

public class Formatter : ITextFormatter
{
	public void Format(LogEvent logEvent, TextWriter output)
	{
		output.Write("[");
		output.Write(logEvent.Level);
		output.Write("] ");

		if (logEvent.Properties.TryGetValue("Context", out var contextValue))
		{
			output.Write("[");
			if (contextValue is ScalarValue sv)
			{
				output.Write(sv.Value);
			}

			output.Write("] ");
		}

		output.Write(logEvent.MessageTemplate);

		if (logEvent.Properties.TryGetValue("StackTrace", out var stackTrace))
		{
			output.WriteLine();
			if (stackTrace is ScalarValue sv)
			{
				output.Write(sv.Value);
			}
		}

		if (logEvent.Exception != null)
		{
			output.WriteLine();
			output.WriteLine(logEvent.Exception.Message);
			output.WriteLine(logEvent.Exception.StackTrace);
		}

		output.WriteLine();
	}
}