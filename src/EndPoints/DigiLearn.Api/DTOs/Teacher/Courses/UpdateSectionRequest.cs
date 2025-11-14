using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Api.DTOs.Teacher.Courses
{
  /// <summary>
  /// Request model for updating a course section
  /// </summary>
  public class UpdateSectionRequest
  {
    /// <summary>
    /// Section title
    /// </summary>
    [Required(ErrorMessage = "Section title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    [Required(ErrorMessage = "Display order is required")]
    [Range(1, 1000, ErrorMessage = "Display order must be between 1 and 1000")]
    public int DisplayOrder { get; set; }
  }

}
