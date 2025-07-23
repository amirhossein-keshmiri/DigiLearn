using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace DigiLearn.Web.TagHelpers
{
  [HtmlTargetElement("editor-row", Attributes = "for")]
  public class EditorRowTagHelper : TagHelper
  {
    public ModelExpression For { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      var metadata = For.Metadata;
      var displayName = metadata.GetDisplayName();
      var isRequired = metadata.IsRequired;
      var name = For.Name;
      var id = TagBuilder.CreateSanitizedId(name, "-");  // Add "-" as the replacement character
      var value = For.Model?.ToString() ?? "";

      // Detect input type based on DataType
      var inputType = metadata.DataTypeName switch
      {
        "EmailAddress" => "email",
        "Password" => "password",
        "PhoneNumber" => "tel",
        _ => "text"
      };

      // Build label
      var labelText = $@"
<label for='{id}' class='form-label'>
    {HtmlEncoder.Default.Encode(displayName)}
    {(isRequired ? "<span class='text-danger'> *</span>" : "")}
</label>";

      // Build input
      var inputValue = inputType != "password" ? value : ""; // Don't echo passwords
      var input = $@"
<input type='{inputType}'
       id='{id}'
       name='{name}'
       value='{HtmlEncoder.Default.Encode(inputValue)}'
       class='form-control'
       {(isRequired ? "required" : "")} />";

      // Build validation
      var validation = $"<span asp-validation-for='{name}' class='text-danger'></span>";

      // Final output
      output.TagName = "div";
      output.Attributes.SetAttribute("class", "form-element-row mb-3");
      output.Content.SetHtmlContent($"{labelText}{input}{validation}");
      output.TagMode = TagMode.StartTagAndEndTag;
    }
  }
}
