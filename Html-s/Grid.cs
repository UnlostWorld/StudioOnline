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

[HtmlTargetElement("Grid")]
public class Grid : Panel
{
	public string? Rows { get; set; }
	public string? Columns { get; set; }

	protected override void Generate()
	{
		base.Generate();

		this.Class("Grid");
		this.Style("grid-template-rows", FormatDefinitions(this.Rows));
		this.Style("grid-template-columns", FormatDefinitions(this.Columns));
	}

	private static string? FormatDefinitions(string? input)
	{
		if (input == null)
			return null;

		input = input.Replace(',', ' ');
		input = input.Replace("min", "min-content");
		input = input.Replace("max", "max-content");
		return input;
	}
}

[HtmlTargetElement("*", ParentTag = "Grid")]
public class GridAttachedProperties : AttachedProperty
{
	[HtmlAttributeName("Grid.Column")]
	public int Column { get; set; } = 0;

	[HtmlAttributeName("Grid.ColumnSpan")]
	public int ColumnSpan { get; set; } = 1;

	[HtmlAttributeName("Grid.Row")]
	public int Row { get; set; } = 0;

	[HtmlAttributeName("Grid.RowSpan")]
	public int RowSpan { get; set; } = 1;

	protected override void Generate()
	{
		this.Class("GridItem");
		this.Style("grid-row", $"{this.Row + 1} / span {this.RowSpan}");
		this.Style("grid-column", $"{this.Column + 1} / span {this.ColumnSpan}");
	}
}