using Common.Application;
using CoreModule.Application.Courses.Create;
using CoreModule.Application.Courses.Edit;
using CoreModule.Application.Courses.Episodes.AddEpisode;
using CoreModule.Application.Courses.Sections.AddSection;
using CoreModule.Query._DTOs;
using CoreModule.Query.Courses.GetByFilter;
using CoreModule.Query.Courses.GetById;
using MediatR;

namespace CoreModule.Facade.Courses
{
  public interface ICourseFacade
  {
    Task<OperationResult> Create(CreateCourseCommand command);
    Task<OperationResult> Edit(EditCourseCommand command);
    Task<OperationResult> AddSection(AddCourseSectionCommand command);
    Task<OperationResult> AddEpisode(AddCourseEpisodeCommand command);

    Task<CourseFilterResult> GetCourseFilter(CourseFilterParams param);
    Task<CourseDto?> GetCourseById(Guid id);
  }

  class CourseFacade : ICourseFacade
  {
    private readonly IMediator _mediator;

    public CourseFacade(IMediator mediator)
    {
      _mediator = mediator;
    }

    public async Task<OperationResult> Create(CreateCourseCommand command)
    {
      return await _mediator.Send(command);
    }

    public async Task<OperationResult> Edit(EditCourseCommand command)
    {
      return await _mediator.Send(command);
    }
    public async Task<OperationResult> AddSection(AddCourseSectionCommand command)
    {
      return await _mediator.Send(command);
    }
    public async Task<OperationResult> AddEpisode(AddCourseEpisodeCommand command)
    {
      return await _mediator.Send(command);
    }

    public async Task<CourseFilterResult> GetCourseFilter(CourseFilterParams param)
    {
      return await _mediator.Send(new GetCoursesByFilterQuery(param));
    }
    public async Task<CourseDto?> GetCourseById(Guid id)
    {
      return await _mediator.Send(new GetCourseByIdQuery(id));
    }
  }
}
