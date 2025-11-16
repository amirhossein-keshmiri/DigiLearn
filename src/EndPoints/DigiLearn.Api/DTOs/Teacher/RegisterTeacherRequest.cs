using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Api.DTOs.Teacher
{
  public class RegisterTeacherRequest
  {
    [Display(Name = "Resume (extension must be pdf)")]
    [Required(ErrorMessage = "Please upload Resume.")]
    public IFormFile CvFile { get; set; }

    [Display(Name = "UserName")]
    [Required(ErrorMessage = "Please enter {0}")]
    [StringLength(100, MinimumLength = 3)]
    public string UserName { get; set; }
  }
}
