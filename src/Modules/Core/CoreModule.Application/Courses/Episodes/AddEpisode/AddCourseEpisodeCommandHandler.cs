using Common.Application;
using Common.Application.FileUtil;
using Common.Application.FileUtil.Interfaces;
using Common.Domain.Utils;
using CoreModule.Application._Utilities;
using CoreModule.Domain.Courses.Models;
using CoreModule.Domain.Courses.Repositories;

namespace CoreModule.Application.Courses.Episodes.AddEpisode
{
  public class AddCourseEpisodeCommandHandler : IBaseCommandHandler<AddCourseEpisodeCommand>
  {
    private readonly ICourseRepository _repository;
    private readonly IFtpFileService _ftpFileService;
    private readonly ILocalFileService _localFileService;

    public AddCourseEpisodeCommandHandler(ICourseRepository repository, IFtpFileService ftpFileService, ILocalFileService localFileService)
    {
      _repository = repository;
      _ftpFileService = ftpFileService;
      _localFileService = localFileService;
    }

    public async Task<OperationResult> Handle(AddCourseEpisodeCommand request, CancellationToken cancellationToken)
    {
      var course = await _repository.GetTracking(request.CourseId);
      if (course == null)
      {
        return OperationResult.NotFound();
      }

      string attExName = null;
      if (request.AttachmentFile != null && request.AttachmentFile.IsValidCompressFile())
        attExName = Path.GetExtension(request.AttachmentFile.FileName);

      var episode = course.AddEpisode(request.SectionId, request.Title, request.EnglishTitle.ToSlug(), Guid.NewGuid(), 
           request.TimeSpan, Path.GetExtension(request.VideoFile.FileName), attExName, request.IsActive);

      await SaveFiles(request, episode);
      await _repository.Save();
      return OperationResult.Success();
    }

    private async Task SaveFiles(AddCourseEpisodeCommand request, Episode episode)
    {
      await _localFileService.SaveFile(request.VideoFile,
          CoreModuleDirectories.CourseEpisode(request.CourseId, episode.Token), episode.VideoName);

      if (request.AttachmentFile != null)
      {
        if (request.AttachmentFile.IsValidCompressFile())
        {
          await _localFileService.SaveFile(request.VideoFile,
              CoreModuleDirectories.CourseEpisode(request.CourseId, episode.Token), episode.AttachmentName!);
        }
      }
    }
  }
}
