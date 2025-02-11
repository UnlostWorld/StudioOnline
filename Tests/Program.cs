// See https://aka.ms/new-console-template for more information
using StudioServer.Client;
using StudioServer.Client.Analytics;

Console.WriteLine("Select a Server:");
Console.WriteLine("1) unlostworld.duckdns.org");
Console.WriteLine("2) localhost");

string? selection = Console.ReadLine();
if (selection == "2")
{
	StudioServerApi.Url = "https://127.0.0.1:7200/api/";
}

Console.WriteLine("Running Tests...");

try
{
	ErrorReport report = new();
	report.Message = new NotSupportedException().Message;
	////report.LogFile = File.ReadAllText("test.log");
	string shortCode = await report.Send();
	Console.WriteLine($"> {shortCode}");

	/*AnalyticEvent evt = new();
	evt.Event = AnalyticEvents.StudioStarted;
	await evt.Send();*/

}
catch(Exception ex)
{
	Console.WriteLine(ex.ToString());
}

Console.WriteLine("Tests Completed. Press 'Q' to terminate.");

ConsoleKeyInfo info;
do
{
	info = Console.ReadKey(true);
}
while (info.Key != ConsoleKey.Q);
