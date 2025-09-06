using Common.Application;
using Microsoft.AspNetCore.Http;

namespace CoreModule.Application.Teachers.Register
{
  public class RegisterTeacherCommand : IBaseCommand
  {
    public IFormFile CvFile { get; set; }
    public string UserName { get; set; }
    public Guid UserId { get; set; }
  }
}
