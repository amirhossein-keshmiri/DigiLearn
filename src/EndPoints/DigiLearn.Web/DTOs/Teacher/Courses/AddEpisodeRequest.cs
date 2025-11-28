using DigiLearn.Web.Infrastructure.Utils.CustomValidation.IFormFile;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.DTOs.Teacher.Courses
{
  public class AddEpisodeRequest
  {
    [Display(Name = "Episode Title")]
    [Required(ErrorMessage = "{0} is required")]
    public string Title { get; set; }

    [Display(Name = "Episode English Title")]
    [Required(ErrorMessage = "{0} is required")]
    public string EnglishTitle { get; set; }

    [Display(Name = "Episode Duration")]
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression(@"^([0-9]{1}|(?:0[0-9]|1[0-9]|2[0-3])+):([0-5]?[0-9])(?::([0-5]?[0-9])(?:.(\d{1,9}))?)?$", ErrorMessage = "Please enter the duration in the correct format.")]
    public TimeSpan TimeSpan { get; set; }

    [Display(Name = "Episode Video")]
    [FileType("mp4", ErrorMessage = "The video is invalid.")]
    [Required(ErrorMessage = "{0} is required")]
    public IFormFile VideoFile { get; set; }

    [Display(Name = "Episode Attachment")]
    [FileType("rar", ErrorMessage = "The attachment is invalid.")]
    public IFormFile? AttachmentFile { get; set; }
  }
}
