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
	protected override void Generate(Generator generator)
	{
	}
}

/// <summary>
/// Base class for all HTMLS elements.
/// </summary>
public abstract class ElementBase : TagHelper
{
	private readonly Generator generator = new();

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		this.generator.Initialize(context, output);

		this.generator.Debug("Type", this.GetType().Name);

		this.Generate(this.generator);
		this.generator.WriteStyle();
		base.Process(context, output);
	}

	protected abstract void Generate(Generator generator);
}

public class Generator
{
	private readonly Dictionary<string, string> styleProperties = new();
	private TagHelperContext? context;
	private TagHelperOutput? output;

	public void Initialize(TagHelperContext context, TagHelperOutput output)
	{
		this.context = context;
		this.output = output;
		this.styleProperties.Clear();
	}

	public void WriteStyle()
	{
		if (this.output == null)
			return;

		if (this.styleProperties.Count > 0)
		{
			StringBuilder styleBuilder = new();

			this.output.Attributes.TryGetAttribute("style", out var existingStyle);
			if (existingStyle != null)
			{
				styleBuilder.Append(existingStyle.Value);
				this.output.Attributes.Remove(existingStyle);
			}

			foreach((string key, string value) in this.styleProperties)
			{
				styleBuilder.Append(key);
				styleBuilder.Append(":");
				styleBuilder.Append(value);
				styleBuilder.Append(";");
			}

			this.output.Attributes.Add("style", styleBuilder.ToString());
		}
	}

	public void HtmlTag(string tag)
	{
		if (this.output == null)
			return;

		this.output.TagName = tag;
	}

	public void HtmlTagMode(TagMode mode)
	{
		if (this.output == null)
			return;

		this.output.TagMode = mode;
	}

	public void Style(string key, string? value)
	{
		if (string.IsNullOrEmpty(value))
			return;

		// replace {X} with var(--X)
		if (value.StartsWith('{') && value.EndsWith('}'))
		{
			value = value.Substring(1, value.Length - 2);
			value = $"var(--{value})";
		}
		else
		{
			// replace , with ' '
			value = value.Replace(", ", " ");
		}

		this.styleProperties[key] = value;
	}

	public void Debug(string key, string message)
	{
		this.PreContent($"<!--  {key}: {message}  -->");
	}

	public void Class(string className)
	{
		if (this.output == null)
			return;

		TagBuilder tb = new("elementGenerator");
		tb.AddCssClass(className);
		this.output.MergeAttributes(tb);
	}

	public void PreContent(string content)
	{
		if (this.output == null)
			return;

		this.output.PreContent.AppendHtml(content);
	}

	public void Content(string content)
	{
		if (this.output == null)
			return;

		this.output.Content.AppendHtml(content);
	}

	public void PostContent(string content)
	{
		if (this.output == null)
			return;

		this.output.PostContent.AppendHtml(content);
	}

	public void Attribute(string key, string? value)
	{
		if (value == null || this.output == null)
			return;

		this.output.Attributes.Add(new TagHelperAttribute(key, value));
	}
}