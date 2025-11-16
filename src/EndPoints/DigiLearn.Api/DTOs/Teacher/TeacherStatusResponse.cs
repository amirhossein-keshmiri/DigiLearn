using CoreModule.Domain.Teachers.Enums;

namespace DigiLearn.Api.DTOs.Teacher
{
  public class TeacherStatusResponse
  {
    public bool IsTeacher { get; set; }
    public TeacherStatus? Status { get; set; }
    public Guid? TeacherId { get; set; }
    public string StatusMessage { get; set; }
  }
}
