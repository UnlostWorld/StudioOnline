namespace StudioServer;

using System;

public static class ShortCodeGenerator
{
	private static readonly char[] ValidChars =
		['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

	private static byte counter = 0;

	// Generates a shortcode in the format XXX-XXX. can generate 33 codes per second without collisions. Codes reset at the end of the year.
	public static string Generate()
	{
		counter++;

		if (counter > ValidChars.Length)
			counter = 0;

		DateTime dt = DateTime.UtcNow;

		char month = ValidChars[dt.Month];
		char day = ValidChars[dt.Day];
		char hour = ValidChars[dt.Hour];

		char minute1 = 'Z';
		char minute2 = 'Z';

		if (dt.Minute < 30)
		{
			minute1 = ValidChars[dt.Minute];
		}
		else
		{
			minute2 = ValidChars[dt.Minute - 30];
		}

		char count = ValidChars[counter];

		return $"{month}{day}{hour}-{minute1}{minute2}{count}";
	}
}
