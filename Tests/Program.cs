// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Console.WriteLine("Tests Completed. Press 'Q' to terminate.");

ConsoleKeyInfo info;
do
{
	info = Console.ReadKey(true);
}
while (info.Key != ConsoleKey.Q);