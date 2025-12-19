using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CoreModule.Application.Courses.Edit
{
  public class EditCourseCommandValidator : AbstractValidator<EditCourseCommand>
  {
    public EditCourseCommandValidator()
    {
      RuleFor(r => r.Title)
          .NotNull()
          .NotEmpty();

      RuleFor(r => r.Slug)
          .NotNull()
          .NotEmpty();

      RuleFor(r => r.Description)
          .NotNull()
          .NotEmpty();

      RuleFor(r => r.ImageFile)
       .Must(BeAValidImage)
       .When(r => r.ImageFile != null)
       .WithMessage("فایل تصویر نامعتبر است");

      RuleFor(r => r.VideoFile)
          .Must(BeAValidVideo)
          .When(r => r.VideoFile != null)
          .WithMessage("فایل ویدیو نامعتبر است");

      RuleFor(r => r.Price)
          .GreaterThanOrEqualTo(0).WithMessage("قیمت باید بزرگتر یا مساوی صفر باشد");

      RuleFor(r => r.CategoryId)
          .NotEqual(Guid.Empty).WithMessage("دسته بندی اصلی را انتخاب کنید");

      RuleFor(r => r.SubCategoryId)
          .NotEqual(Guid.Empty).WithMessage("زیر دسته بندی را انتخاب کنید");

      RuleFor(r => r.CourseId)
          .NotEqual(Guid.Empty).WithMessage("شناسه دوره نامعتبر است");
    }

    private bool BeAValidImage(IFormFile file)
    {
      if (file == null) return true; // Null is OK for edit

      var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
      var extension = Path.GetExtension(file.FileName)?.ToLower();

      return !string.IsNullOrEmpty(extension)
             && validExtensions.Contains(extension)
             && file.Length > 0
             && file.Length <= 5 * 1024 * 1024; // Max 5MB
    }

    private bool BeAValidVideo(IFormFile file)
    {
      if (file == null) return true; // Null is OK for edit

      var extension = Path.GetExtension(file.FileName)?.ToLower();

      return extension == ".mp4"
             && file.Length > 0
             && file.Length <= 100 * 1024 * 1024; // Max 100MB
    }
  }
}
