using Common.Application;
using Common.Application.FileUtil;
using Common.Application.FileUtil.Interfaces;
using Common.Application.SecurityUtil;
using CoreModule.Application._Utilities;
using CoreModule.Domain.Courses.DomainServices;
using CoreModule.Domain.Courses.Repositories;

namespace CoreModule.Application.Courses.Edit
{
  public class EditCourseCommandHandler : IBaseCommandHandler<EditCourseCommand>
  {
    private readonly ILocalFileService _localFileService;
    private readonly ICourseRepository _repository;
    private readonly ICourseDomainService _domainService;
    public EditCourseCommandHandler(ILocalFileService localFileService, ICourseDomainService domainService, ICourseRepository repository)
    {
      _localFileService = localFileService;
      _domainService = domainService;
      _repository = repository;
    }

    public async Task<OperationResult> Handle(EditCourseCommand request, CancellationToken cancellationToken)
    {
      var course = await _repository.GetTracking(request.CourseId);
      if (course == null)
      {
        return OperationResult.NotFound();
      }

      var imageName = course.ImageName;
      string? videoPath = course.VideoName;
      var oldVideoFileName = course.VideoName;
      var oldImageFileName = course.ImageName;

      if (request.VideoFile != null)
      {
        if (request.VideoFile.IsValidMp4File() == false)
        {
          return OperationResult.Error("فایل وارد شده نامعتبر است");
        }

        videoPath = await _localFileService.SaveFileAndGenerateName(request.VideoFile, CoreModuleDirectories.CourseDemo(course.Id));
      }

      if (request.ImageFile != null && request.ImageFile.IsImage())
      {
        imageName = await _localFileService.SaveFileAndGenerateName(request.ImageFile!, CoreModuleDirectories.CourseImage);
      }

      course.Edit(request.Title, request.Description, imageName, videoPath,
          request.Price,
          request.SeoData, request.CourseLevel, request.CourseStatus, request.CategoryId, request.SubCategoryId, request.Slug, request.CourseActionStatus,
          _domainService);

      await _repository.Save();

      DeleteOldFiles(oldImageFileName, oldVideoFileName,
         request.VideoFile != null,
         request.ImageFile != null, course);

      return OperationResult.Success();
    }

    void DeleteOldFiles(string image, string? video, bool isUploadNewVideo, bool isUploadNewImage, Domain.Courses.Models.Course course)
    {
      if (isUploadNewVideo && string.IsNullOrWhiteSpace(video) == false)
      {
        _localFileService.DeleteFile(CoreModuleDirectories.CourseDemo(course.Id), video);
      }

      if (isUploadNewImage && !string.IsNullOrWhiteSpace(image))
      {
        _localFileService.DeleteFile(CoreModuleDirectories.CourseImage, image);
      }
    }
  }
}
