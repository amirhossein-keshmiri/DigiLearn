using CoreModule.Domain.Courses.Enums;
using DigiLearn.Api.Infrastructure.Utils.CustomValidation.IFormFile;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Api.DTOs.Teacher.Courses
{
  public class CreateCourseRequest
  {
    /// <summary>
    /// Main category ID
    /// </summary>
    [Display(Name = "Category")]
    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Sub-category ID
    /// </summary>
    [Display(Name = "Sub-Category")]
    [Required(ErrorMessage = "Sub-category is required")]
    public Guid SubCategoryId { get; set; }

    /// <summary>
    /// Course title
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; }

    /// <summary>
    /// URL-friendly slug (will be converted to lowercase with hyphens)
    /// </summary>
    [Required(ErrorMessage = "Slug is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Slug must be between 3 and 200 characters")]
    [RegularExpression(@"^[a-zA-Z0-9-]+$", ErrorMessage = "Slug can only contain letters, numbers, and hyphens")]
    public string Slug { get; set; }

    /// <summary>
    /// Course description (HTML supported)
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    [MinLength(50, ErrorMessage = "Description must be at least 50 characters")]
    public string Description { get; set; }

    /// <summary>
    /// Course thumbnail image (Required, JPG/PNG/WebP, max 5MB)
    /// </summary>
    [Display(Name = "Image Name")]
    [Required(ErrorMessage = "Course image is required")]
    [FileImage(ErrorMessage = "Not supported image.")]
    public IFormFile ImageFile { get; set; }

    /// <summary>
    /// Course introduction video (Optional, MP4, max 100MB)
    /// </summary>
    [Display(Name = "Video Intro Name")]
    [FileType("mp4", ErrorMessage = "Not supported video.")]
    public IFormFile? VideoFile { get; set; }

    /// <summary>
    /// Course price (0 for free courses)
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
