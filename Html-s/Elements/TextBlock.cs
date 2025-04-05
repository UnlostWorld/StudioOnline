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

[HtmlTargetElement("TextBlock")]
public class TextBlock : Element
{
	public string Text { get; set; } = string.Empty;
	public string Alignment { get; set; } = "Left";

	protected override void Generate(Generator generator)
	{
		base.Generate(generator);
		generator.Class("TextBlock");
		generator.HtmlTag("div");
		generator.HtmlTagMode(TagMode.StartTagAndEndTag);
		generator.Style("text-align", this.Alignment);
		generator.Content(this.Text);
	}
}