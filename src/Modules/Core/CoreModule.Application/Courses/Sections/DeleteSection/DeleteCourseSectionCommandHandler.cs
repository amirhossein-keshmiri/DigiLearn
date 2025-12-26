using Common.Application;
using CoreModule.Domain.Courses.Repositories;

namespace CoreModule.Application.Courses.Sections.DeleteSection
{
  class DeleteCourseSectionCommandHandler : IBaseCommandHandler<DeleteCourseSectionCommand>
  {
    private readonly ICourseRepository _repository;

    public DeleteCourseSectionCommandHandler(ICourseRepository repository)
    {
      _repository = repository;
    }
    public async Task<OperationResult> Handle(DeleteCourseSectionCommand request, CancellationToken cancellationToken)
    {
      var course = await _repository.GetTracking(request.CourseId);
      if (course == null)
      {
        return OperationResult.NotFound();
      }

      var section = course.Sections.FirstOrDefault(f => f.Id == request.SectionId);
      if (section == null)
      {
        return OperationResult.NotFound();
      }

      if (section!.Episodes.Count > 0 || section!.Episodes.Any())
      {
        return OperationResult.Error("You cannot delete this Section, because it have episodes. You must first remove it's Episodes!");
      }

      course.RemoveSection(request.SectionId);
      await _repository.Save();
      return OperationResult.Success();
    }
  }
}
