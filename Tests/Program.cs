// See https://aka.ms/new-console-template for more information
using StudioServer.Client;

try
{
	ErrorReport report = new();
	report.Message = "Test!";
	string shortCode = await report.Send();
	Console.WriteLine($"> {shortCode}");

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
