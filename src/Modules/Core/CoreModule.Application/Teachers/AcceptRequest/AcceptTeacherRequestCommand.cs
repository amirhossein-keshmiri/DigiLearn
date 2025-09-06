using Common.Application;

namespace CoreModule.Application.Teachers.AcceptRequest
{
  public class AcceptTeacherRequestCommand : IBaseCommand
  {
    public Guid TeacherId { get; set; }
  }
}
