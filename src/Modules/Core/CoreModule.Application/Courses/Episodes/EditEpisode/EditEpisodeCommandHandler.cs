using Common.Application;
using Common.Application.FileUtil;
using Common.Application.FileUtil.Interfaces;
using CoreModule.Application._Utilities;
using CoreModule.Domain.Courses.Models;
using CoreModule.Domain.Courses.Repositories;
using Microsoft.AspNetCore.Http;

namespace CoreModule.Application.Courses.Episodes.EditEpisode
{
  class EditEpisodeCommandHandler : IBaseCommandHandler<EditEpisodeCommand>
  {
    private readonly ICourseRepository _repository;
    private readonly ILocalFileService _localFileService;

    public EditEpisodeCommandHandler(ICourseRepository repository, ILocalFileService localFileService)
    {
      _repository = repository;
      _localFileService = localFileService;
    }

    public async Task<OperationResult> Handle(EditEpisodeCommand request, CancellationToken cancellationToken)
    {
      var course = await _repository.GetTracking(request.CourseId);
      if (course == null)
      {
        return OperationResult.NotFound();
      }

      var episode = course.GetEpisodeById(request.SectionId, request.Id);

      if (episode == null)
      {
        return OperationResult.NotFound();
      }

      string? attname = null;
      if (request.AttachmentFile != null)
      {
        await SaveAttachment(request.AttachmentFile, episode, course.Id);
      }
      if (request.VideoFile != null)
      {
        await SaveVideoFile(request.VideoFile, episode, course.Id);
      }

      course.EditEpisode(request.Id, request.SectionId, request.Title, request.IsActive, request.TimeSpan, attname);
      await _repository.Save();
      return OperationResult.Success();
    }

    private async Task<string?> SaveAttachment(IFormFile attachment, Episode episode, Guid courseId)
    {
      if (attachment.IsValidCompressFile())
      {
        ////This Will Save the Attachment with the uploaded name
        //await _localFileService.SaveFile(attachment, CoreModuleDirectories.CourseEpisode(courseId, episode.Token),
        //    attachment.FileName);
        //return attachment.FileName;

        ////This Will Save the Attachment with the episode video name with IsValidCompressFile extension
        var attName = episode.VideoName.Replace(".mp4", Path.GetExtension(attachment.FileName));
        await _localFileService.SaveFile(attachment, CoreModuleDirectories.CourseEpisode(courseId, episode.Token),
            attName);
        return attName;
      }

      return null;
    }

    private async Task SaveVideoFile(IFormFile videoFile, Episode episode, Guid courseId)
    {
      if (videoFile.IsValidMp4File())
      {
        await _localFileService.SaveFile(videoFile, CoreModuleDirectories.CourseEpisode(courseId, episode.Token),
            episode.VideoName);
      }
    }
  }
}
