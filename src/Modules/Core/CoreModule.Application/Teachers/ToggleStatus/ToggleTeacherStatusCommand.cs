using Common.Application;

namespace CoreModule.Application.Teachers.ToggleStatus
{
  public record ToggleTeacherStatusCommand(Guid TeacherId) : IBaseCommand;
}
