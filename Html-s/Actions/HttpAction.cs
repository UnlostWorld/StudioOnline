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

public enum HttpMethods
{
	Get,
	Post,
}

public class HttpAction(string path, HttpMethods method = HttpMethods.Get, string? name = null, string? value = null)
	: ActionHandler
{
	public override void Generate(Generator generator)
	{
		base.Generate(generator);

		generator.HtmlTag("form");
		generator.Attribute("action", path);
		generator.Attribute("method", method.ToString().ToLower());

		generator.PreContent($"<button type=\"submit\" name=\"{name}\" value=\"{value}\">");
		generator.PostContent("</button>");
	}
}
