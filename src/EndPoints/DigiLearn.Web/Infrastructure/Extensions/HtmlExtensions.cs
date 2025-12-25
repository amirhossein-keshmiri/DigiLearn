using System.Text.RegularExpressions;

namespace DigiLearn.Web.Infrastructure.Extensions
{
  public static class HtmlExtensions
  {
    public static string StripHtml(this string input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return string.Empty;

      return Regex.Replace(input, "<.*?>", string.Empty);
    }
  }
}
