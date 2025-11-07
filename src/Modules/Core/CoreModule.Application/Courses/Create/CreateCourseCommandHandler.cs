using Common.Application;
using Common.Application.FileUtil;
using Common.Application.FileUtil.Interfaces;
using CoreModule.Application._Utilities;
using CoreModule.Domain.Courses.DomainServices;
using CoreModule.Domain.Courses.Repositories;

namespace CoreModule.Application.Courses.Create
{
  public class CreateCourseCommandHandler : IBaseCommandHandler<CreateCourseCommand>
  {
    private readonly IFtpFileService _ftpFileService;
    private readonly ILocalFileService _localFileService;
    private readonly ICourseRepository _repository;
    private readonly ICourseDomainService _domainService;
    public CreateCourseCommandHandler(IFtpFileService fileService, ILocalFileService localFileService, ICourseDomainService domainService, ICourseRepository repository)
    {
      _ftpFileService = fileService;
      _localFileService = localFileService;
      _domainService = domainService;
      _repository = repository;
    }

    public async Task<OperationResult> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
      var imageName = await _localFileService.SaveFileAndGenerateName(request.ImageFile, CoreModuleDirectories.CourseImage);

      string videoPath = null;
      Guid courseId = Guid.NewGuid();
      if (request.VideoFile != null)
      {
        if (request.VideoFile.IsValidMp4File() == false)
        {
          return OperationResult.Error("فایل وارد شده نامعتبر است");
        }

        //videoPath = await _ftpFileService.SaveFileAndGenerateName(request.VideoFile, CoreModuleDirectories.CourseDemo(courseId));
        videoPath = await _localFileService.SaveFileAndGenerateName(request.VideoFile, CoreModuleDirectories.CourseDemo(courseId));
      }

      var course = new Domain.Courses.Models.Course(request.TeacherId, request.CategoryId, request.SubCategoryId, 
                                                    request.Title, request.Slug, request.Description, imageName, videoPath,
                                                    request.Price, request.SeoData, request.CourseLevel, request.Status, _domainService)
      {
        Id = courseId
      };

      _repository.Add(course);
      await _repository.Save();
      return OperationResult.Success();
    }
  }
}
