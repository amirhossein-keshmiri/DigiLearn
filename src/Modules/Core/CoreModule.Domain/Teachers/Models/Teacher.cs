using Common.Domain;
using Common.Domain.Exceptions;
using Common.Domain.Utils;
using CoreModule.Domain.Teachers.DomainServices;
using CoreModule.Domain.Teachers.Enums;

namespace CoreModule.Domain.Teachers.Models
{
  public class Teacher : AggregateRoot
  {
    private Teacher()
    {
      
    }
    public Teacher(Guid userId, string userName, string cvFileName, ITeacherDomainService domainService)
    {
      Guard(userName, cvFileName);

      if (userName.IsUniCode())
        throw new InvalidDomainDataException("UserName Invalid");

      if (domainService.UserNameIsExist(userName))
        throw new InvalidDomainDataException("UserName Is Exist");

      UserId = userId;
      UserName = userName.ToLower();
      CvFileName = cvFileName;
      Status = TeacherStatus.Pending;
    }

    public Guid UserId { get; private set; }
    public string UserName { get; private set; }
    public string CvFileName { get; private set; }
    public TeacherStatus Status { get; private set; }

    public void AcceptRequest()
    {
      if (Status == TeacherStatus.Pending)
      {
        //Event

        Status = TeacherStatus.Active;
      }
    }

    public void ToggleStatus()
    {
      if (Status == TeacherStatus.Active)
      {
        Status = TeacherStatus.Inactive;
      }
      else if (Status == TeacherStatus.Inactive)
      {
        Status = TeacherStatus.Active;
      }
    }

    void Guard(string userName, string cvFileName)
    {
      NullOrEmptyDomainDataException.CheckString(userName, nameof(userName));
      NullOrEmptyDomainDataException.CheckString(cvFileName, nameof(cvFileName));
    }
  }
}
