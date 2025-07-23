using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DigiLearn.Web.TagHelpers
{
  [HtmlTargetElement("editor-row", Attributes = "for")]
  public class EditorRowTagHelper : TagHelper
  {
    private readonly IHtmlGenerator _htmlGenerator;

    public EditorRowTagHelper(IHtmlGenerator htmlGenerator)
    {
      _htmlGenerator = htmlGenerator;
    }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    public ModelExpression For { get; set; } = null!;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      if (For == null)
      {
        output.SuppressOutput();
        return;
      }

      var metadata = For.Metadata;
      var displayName = metadata.GetDisplayName();
      var isRequired = metadata.IsRequired;
      var name = For.Name;
      var id = TagBuilder.CreateSanitizedId(name, "-");

      var value = For.Model?.ToString() ?? "";
      var inputType = metadata.DataTypeName switch
      {
        "EmailAddress" => "email",
        "Password" => "password",
        "PhoneNumber" => "tel",
        _ => "text"
      };
      var inputValue = inputType == "password" ? "" : value;

      // Build label
      var label = $@"
<label for='{id}' class='form-label'>
    {displayName}
    {(isRequired ? "<span class='text-danger'> *</span>" : "")}
</label>";

      // Build input
      var input = new TagBuilder("input");
      input.Attributes["type"] = inputType;
      input.Attributes["id"] = id;
      input.Attributes["name"] = name;
      input.Attributes["value"] = inputValue;
      input.AddCssClass("form-control");
      if (isRequired)
      {
        input.Attributes["required"] = "required";
      }

      // Create validation message
      var validationContext = new TagHelperContext(
          new TagHelperAttributeList(),
          new Dictionary<object, object>(),
          Guid.NewGuid().ToString("n"));

      var validationOutput = new TagHelperOutput(
          "span",
          new TagHelperAttributeList(),
          (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

      var validationHelper = new ValidationMessageTagHelper(_htmlGenerator)
      {
        For = For,
        ViewContext = ViewContext
      };

      await validationHelper.ProcessAsync(validationContext, validationOutput);

      // ✅ Manually add class after processing
      var currentClass = validationOutput.Attributes["class"]?.Value?.ToString() ?? "";
      var newClass = string.IsNullOrEmpty(currentClass)
          ? "text-danger"
          : $"{currentClass} text-danger";

      validationOutput.Attributes.SetAttribute("class", newClass);

      // Final output
      output.TagName = "div";
      output.Attributes.SetAttribute("class", "form-element-row mb-3");
      output.Content.SetHtmlContent(label);
      output.Content.AppendHtml(input);
      output.Content.AppendHtml(validationOutput);
      output.TagMode = TagMode.StartTagAndEndTag;
    }
  }
}
