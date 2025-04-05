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

[HtmlTargetElement("*", Attributes = "Height")]
[HtmlTargetElement("*", Attributes = "Width")]
[HtmlTargetElement("*", Attributes = "MaxHeight")]
[HtmlTargetElement("*", Attributes = "MaxWidth")]
[HtmlTargetElement("*", Attributes = "MinHeight")]
[HtmlTargetElement("*", Attributes = "MinWidth")]
public class SizeProperties : AttachedProperty
{
	[HtmlAttributeName("Height")]
	public string? Height { get; set; }

	[HtmlAttributeName("Width")]
	public string? Width { get; set; }

	[HtmlAttributeName("MaxHeight")]
	public string? MaxHeight { get; set; }

	[HtmlAttributeName("MaxWidth")]
	public string? MaxWidth { get; set; }

	[HtmlAttributeName("MinHeight")]
	public string? MinHeight { get; set; }

	[HtmlAttributeName("MinWidth")]
	public string? MinWidth { get; set; }

	protected override void Generate(Generator generator)
	{
		generator.Style("height", this.Height);
		generator.Style("width", this.Width);
		generator.Style("max-height", this.MaxHeight);
		generator.Style("max-width", this.MaxWidth);
		generator.Style("min-height", this.MinHeight);
		generator.Style("min-width", this.MinWidth);
	}
}