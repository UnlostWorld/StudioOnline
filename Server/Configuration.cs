namespace StudioServer;

using Newtonsoft.Json;
using System.IO;

public class Configuration
{
	private const string FileName = "Config.json";

	public Configuration()
	{
		this.Current = new();
		this.Load();
	}

	public Config Current { get; private set; }

	public void Load()
	{
		if (File.Exists(FileName))
		{
			Config? loaded = JsonConvert.DeserializeObject<Config>(File.ReadAllText(FileName));
			this.Current = loaded ?? new();
		}
		else
		{
			this.Current = new();
			this.Save();
		}
	}

	public void Save()
	{
		File.WriteAllText(FileName, JsonConvert.SerializeObject(this.Current, Formatting.Indented));
	}

	public class Config
	{
		public string? DatabaseURI { get; set; }
		public string? BotToken { get; set; }

		public ulong BotErrorReportsChannel { get; set; }
	}
}
