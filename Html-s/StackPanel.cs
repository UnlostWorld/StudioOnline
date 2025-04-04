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

using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("StackPanel")]
public class StackPanelHelper : Panel
{
	public string Orientation { get; set; } = "Horizontal";

	protected override void Generate()
	{
		base.Generate();

		if (this.Orientation == "Horizontal")
		{
			this.Style("grid-auto-flow", "column");
		}
		else if (this.Orientation == "Vertical")
		{
			this.Style("grid-auto-flow", "row");
		}
		else
		{
			throw new Exception($"Invalid Orientation: {this.Orientation}");
		}
	}
}
