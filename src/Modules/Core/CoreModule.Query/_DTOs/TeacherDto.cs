using Common.Query;
using CoreModule.Domain.Teachers.Enums;

namespace CoreModule.Query._DTOs
{
  public class TeacherDto : BaseDto
  {
    public string UserName { get; set; }
    public string CvFileName { get; set; }
    public TeacherStatus Status { get; set; }
    public CoreModuleUserDto User { get; set; }
  }
}
