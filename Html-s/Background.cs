// .                    @@             _____ _______ _    _ _____ _____ ____
//          @       @@@@@             / ____|__   __| |  | |  __ \_   _/ __ \
//         @@@  @@@@                 | (___    | |  | |  | | |  | || || |  | |
//         @@@@@@@@@  @    @          \___ \   | |  | |  | | |  | || || |  | |
//        @@@@       @@@@@@@          ____) |  | |  | |__| | |__| || || |__| |
//    @@@@@             @@@          |_____/   |_|   \____/|_____/_____\____/
//     @@@      @@@@@@@  @@                   __                    ___
//      @@   @@@@    @@  @@                  /  \ |\ | |    | |\ | |__
//      @@  @@@     @@   @   @               \__/ | \| |___ | | \| |___
//    @@@@  @@@   @@@@   @@@@
//     @@@@  @@@@@  @@@@@@@         https://github.com/UnlostWorld/StudioOnline
//       @@@
//        @@@@@@@@@@@@@@                This software is licensed under the
//            @@@@  @                  GNU AFFERO GENERAL PUBLIC LICENSE v3

namespace HtmlS;

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("*", Attributes = "Background")]
public class BackgroundTagHelper : AttachedProperty
{
	public string? Background { get; set; }

	protected override void Generate()
	{
		base.Generate();

		if (this.Background == null)
			return;

		string value = this.Background;

		// replace {} with var()
		if (value.StartsWith('{') && value.EndsWith('}'))
		{
			value = value.Substring(1, value.Length - 2);
			value = $"var({value})";
		}

		this.Style("background", value);
	}
}