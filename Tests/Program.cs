// See https://aka.ms/new-console-template for more information
using StudioServer.Client;

Console.WriteLine("Select a Server:");
Console.WriteLine("1) unlostworld.duckdns.org");
Console.WriteLine("2) localhost");

string? selection = Console.ReadLine();
if (selection == "2")
{
	StudioServer.Client.StudioServer.Url = "http://127.0.0.1:5202/api/";
}

Console.WriteLine("Running Tests...");

try
{
	/*ErrorReport report = new();
	report.Message = new NotSupportedException().Message;
	report.LogFile = File.ReadAllText("test.log");
	string shortCode = await report.Send();
	Console.WriteLine($"> {shortCode}");*/

	AnalyticEvent evt = new();
	evt.EventName = "Startup";
	await evt.Send();

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
