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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

/// <summary>
/// A button is a control that recieves mouse click events.
/// </summary>
[HtmlTargetElement("Button")]
public class Button : Panel
{
	public enum HttpMethods
	{
		Get,
		Post,
	}

	public string? Path { get; set; }
	public HttpMethods Method { get; set; } = HttpMethods.Get;
	public string? Name { get; set; } = null;
	public string? Value { get; set; } = null;

	protected override void Generate(Generator generator)
	{
		base.Generate(generator);

		generator.HtmlTag("form");
		generator.Attribute("action", this.Path);
		generator.Attribute("method", this.Method.ToString().ToLower());

		generator.PreContent($"<button class=\"{this.GetHtmlButtonClasses()}\" type=\"submit\" name=\"{this.Name}\" value=\"{this.Value}\">");
		generator.PostContent("</button>");
	}

	protected virtual string GetHtmlButtonClasses()
	{
		return "Button";
	}
}