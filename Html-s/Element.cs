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

using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

public abstract class Element : ElementBase
{
	public string? Width { get; set; }
	public string? Height { get; set; }

	protected override void Generate()
	{
		this.Style("Width", this.Width);
		this.Style("Height", this.Height);
	}
}

/// <summary>
/// Base class for all HTMLS elements.
/// </summary>
public abstract class ElementBase : TagHelper
{
	private readonly Dictionary<string, string> styleProperties = new();
	private TagHelperContext? generateContext;
	private TagHelperOutput? generateOutput;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		this.generateContext = context;
		this.generateOutput = output;
		this.styleProperties.Clear();

		this.Generate();

		if (this.styleProperties.Count > 0)
		{
			StringBuilder styleBuilder = new();

			output.Attributes.TryGetAttribute("style", out var existingStyle);
			if (existingStyle != null)
			{
				styleBuilder.Append(existingStyle.Value);
				output.Attributes.Remove(existingStyle);
			}

			foreach((string key, string value) in this.styleProperties)
			{
				styleBuilder.Append(key);
				styleBuilder.Append(":");
				styleBuilder.Append(value);
				styleBuilder.Append(";");
			}

			output.Attributes.Add("style", styleBuilder.ToString());
		}

		base.Process(context, output);

		this.generateContext = null;
		this.generateOutput = null;
	}

	protected abstract void Generate();

	protected void HtmlTag(string tag)
	{
		if (this.generateOutput == null)
			return;

		this.generateOutput.TagName = tag;
	}

	protected void HtmlTagMode(TagMode mode)
	{
		if (this.generateOutput == null)
			return;

		this.generateOutput.TagMode = mode;
	}

	protected void Style(string key, string? value)
	{
		if (string.IsNullOrEmpty(value))
			return;

		this.styleProperties[key] = value;
	}

	protected void Class(string className)
	{
		TagBuilder tb = new("elementGenerator");
		tb.AddCssClass(className);
		this.generateOutput.MergeAttributes(tb);
	}

	protected void Content(string content)
	{
		if (this.generateOutput == null)
			return;

		this.generateOutput.Content.Append(content);
	}

	protected void Attribute(string key, string? value)
	{
		if (value == null || this.generateOutput == null)
			return;

		this.generateOutput.Attributes.Add(new TagHelperAttribute(key, value));
	}
}
