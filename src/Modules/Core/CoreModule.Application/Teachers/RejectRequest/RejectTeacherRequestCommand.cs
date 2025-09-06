using Common.Application;

namespace CoreModule.Application.Teachers.RejectRequest
{
  public class RejectTeacherRequestCommand : IBaseCommand
  {
    public Guid TeacherId { get; set; }
    public string Description { get; set; }
  }
}
