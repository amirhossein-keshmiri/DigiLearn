using CoreModule.Domain.Courses.Enums;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Api.DTOs.Teacher.Courses
{
  /// <summary>
  /// Request model for updating an existing course
  /// </summary>
  public class UpdateCourseRequest
  {
    /// <summary>
    /// Main category ID
    /// </summary>
    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Sub-category ID
    /// </summary>
    [Required(ErrorMessage = "Sub-category is required")]
    public Guid SubCategoryId { get; set; }

    /// <summary>
    /// Course title
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; }

    /// <summary>
    /// URL-friendly slug
    /// </summary>
    [Required(ErrorMessage = "Slug is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Slug must be between 3 and 200 characters")]
    [RegularExpression(@"^[a-zA-Z0-9-]+$", ErrorMessage = "Slug can only contain letters, numbers, and hyphens")]
    public string Slug { get; set; }

    /// <summary>
    /// Course description
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    [MinLength(50, ErrorMessage = "Description must be at least 50 characters")]
    public string Description { get; set; }

    /// <summary>
    /// New course image (Optional - leave empty to keep current image)
    /// </summary>
    public IFormFile? ImageFile { get; set; }

    /// <summary>
    /// New introduction video (Optional - leave empty to keep current video)
    /// </summary>
    public IFormFile? VideoFile { get; set; }

    /// <summary>
    /// Course price
    /// </summary>
    [Required(ErrorMessage = "Price is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Price must be 0 or greater")]
    public int Price { get; set; }

    /// <summary>
    /// Course difficulty level
    /// </summary>
    [Required(ErrorMessage = "Course level is required")]
    public CourseLevel CourseLevel { get; set; }
  }
}
