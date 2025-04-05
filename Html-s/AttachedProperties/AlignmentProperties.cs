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

public enum HorizontalAlignments
{
	Left,
	Center,
	Stretch,
	Right,
}

public enum VerticalAlignments
{
	Top,
	Center,
	Stretch,
	Bottom,
}

[HtmlTargetElement("*", Attributes = "HorizontalAlignment")]
[HtmlTargetElement("*", Attributes = "VerticalAlignment")]
public class HorizontalAlignmentProperty : AttachedProperty
{
	[HtmlAttributeName("HorizontalAlignment")]
	public HorizontalAlignments? HorizontalAlignment { get; set; }

	[HtmlAttributeName("VerticalAlignment")]
	public VerticalAlignments? VerticalAlignment { get; set; }

	protected override void Generate(Generator generator)
	{
		if (this.HorizontalAlignment == HorizontalAlignments.Left)
		{
			generator.Style("margin-left", "0%");
			generator.Style("margin-right", "auto");
		}
		else if (this.HorizontalAlignment == HorizontalAlignments.Center)
		{
			generator.Style("margin-left", "auto");
			generator.Style("margin-right", "auto");
			generator.Style("width", "100%");
		}
		else if (this.HorizontalAlignment == HorizontalAlignments.Stretch)
		{
			generator.Style("margin-left", "0%");
			generator.Style("margin-right", "0%");
			generator.Style("width", "100%");
		}
		else if (this.HorizontalAlignment == HorizontalAlignments.Right)
		{
			generator.Style("margin-left", "auto");
			generator.Style("margin-right", "0%");
		}

		if (this.VerticalAlignment == VerticalAlignments.Top)
		{
			generator.Style("margin-top", "0%");
			generator.Style("margin-bottom", "auto");
		}
		else if (this.VerticalAlignment == VerticalAlignments.Center)
		{
			generator.Style("margin-top", "auto");
			generator.Style("margin-bottom", "auto");
			generator.Style("height", "100%");
		}
		else if (this.VerticalAlignment == VerticalAlignments.Stretch)
		{
			generator.Style("margin-top", "0%");
			generator.Style("margin-bottom", "0%");
			generator.Style("height", "100%");
		}
		else if (this.VerticalAlignment == VerticalAlignments.Bottom)
		{
			generator.Style("margin-top", "auto");
			generator.Style("margin-bottom", "0%");
		}
	}
}