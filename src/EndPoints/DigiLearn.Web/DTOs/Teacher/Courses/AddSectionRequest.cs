using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.DTOs.Teacher.Courses
{
  /// <summary>
  /// Request model for adding a section to a course
  /// </summary>
  public class AddSectionRequest
  {
    /// <summary>
    /// Section title
    /// </summary>
    [Required(ErrorMessage = "Section title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; }

    /// <summary>
    /// Display order (determines section position in the course)
    /// </summary>
    [Required(ErrorMessage = "Display order is required")]
    [Range(1, 1000, ErrorMessage = "Display order must be between 1 and 1000")]
    public int DisplayOrder { get; set; }
  }
}
