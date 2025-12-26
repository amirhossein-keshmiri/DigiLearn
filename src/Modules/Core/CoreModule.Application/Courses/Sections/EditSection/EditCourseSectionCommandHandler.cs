using Common.Application;
using CoreModule.Domain.Courses.Repositories;

namespace CoreModule.Application.Courses.Sections.EditSection
{
    class EditCourseSectionCommandHandler : IBaseCommandHandler<EditCourseSectionCommand>
    {
        private readonly ICourseRepository _repository;

        public EditCourseSectionCommandHandler(ICourseRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult> Handle(EditCourseSectionCommand request, CancellationToken cancellationToken)
        {
            var course = await _repository.GetTracking(request.CourseId);
            if (course == null)
            {
                return OperationResult.NotFound();
            }

            if (course.Sections.Any(f => f.DisplayOrder == request.DisplayOrder & f.Id != request.Id))
            {
                return OperationResult.Error("Display Order Is Exist for another section order.");
            }

            //var section = course.Sections.FirstOrDefault(f => f.Id == request.Id);

            var section = course.GetSectionById(request.Id);
            if (section == null)
            {
                return OperationResult.NotFound();
            }

            course.EditSection(request.Id, request.DisplayOrder, request.Title);
            await _repository.Save();
            return OperationResult.Success();
        }
    }
}
