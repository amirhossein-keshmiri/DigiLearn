using CoreModule.Application.Teachers.Register;
using CoreModule.Domain.Teachers.Enums;
using CoreModule.Facade.Teachers;
using CoreModule.Query._DTOs;
using DigiLearn.Api.DTOs.Teacher;
using DigiLearn.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiLearn.Api.Controllers
{
  /// <summary>
  /// Teacher Management API
  /// </summary>
  [Authorize]
  public class TeacherController : ApiController
  {
    private readonly ITeacherFacade _teacherFacade;

    public TeacherController(ITeacherFacade teacherFacade)
    {
      _teacherFacade = teacherFacade;
    }

    /// <summary>
    /// Get current user's teacher profile
    /// Returns null if user is not a teacher
    /// </summary>
    [HttpGet("CurrentTeacher")]
    [ProducesResponseType(typeof(ApiResult<TeacherDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult<TeacherDto>> GetCurrentTeacher()
    {
      var userId = User.GetUserId();
      var teacher = await _teacherFacade.GetByUserId(userId);

      if (teacher == null)
      {
        return new ApiResult<TeacherDto>
        {
          IsSuccess = false,
          Data = null,
          MetaData = new MetaData
          {
            Message = "Teacher profile not found",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      return QueryResult(teacher);
    }

    /// <summary>
    /// Register current user as a teacher
    /// Requires CV file upload and username
    /// </summary>
    [HttpPost("Register")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ApiResult> RegisterTeacher([FromForm] RegisterTeacherRequest request)
    {
      if (!ModelState.IsValid)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = JoinErrors(),
            AppStatusCode = AppStatusCode.BadRequest
          }
        };
      }

      var userId = User.GetUserId();

      // Check if user is already a teacher
      var existingTeacher = await _teacherFacade.GetByUserId(userId);
      if (existingTeacher != null)
      {
        var message = existingTeacher.Status switch
        {
          TeacherStatus.Active => "You are already registered as an active teacher",
          TeacherStatus.Inactive => "You are already registered as a teacher",
          TeacherStatus.Pending => "Your teacher application is pending approval",
          _ => "You are already registered as a teacher"
        };

        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = message,
            AppStatusCode = AppStatusCode.LogicError
          }
        };
      }

      var command = new RegisterTeacherCommand
      {
        UserId = userId,
        UserName = request.UserName,
        CvFile = request.CvFile
      };

      var result = await _teacherFacade.Register(command);

      //return CommandResult(result, System.Net.HttpStatusCode.Created, $"/api/Teacher/CurrentTeacher");
      return CommandResult(result);
    }

    /// <summary>
    /// Check if current user is an active teacher
    /// </summary>
    [HttpGet("IsActiveTeacher")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    public async Task<ApiResult<bool>> IsActiveTeacher()
    {
      var userId = User.GetUserId();
      var teacher = await _teacherFacade.GetByUserId(userId);

      var isActive = teacher != null && teacher.Status == TeacherStatus.Active;

      return QueryResult(isActive);
    }

    /// <summary>
    /// Get teacher status for current user
    /// </summary>
    [HttpGet("Status")]
    [ProducesResponseType(typeof(ApiResult<TeacherStatusResponse>), StatusCodes.Status200OK)]
    public async Task<ApiResult<TeacherStatusResponse>> GetTeacherStatus()
    {
      var userId = User.GetUserId();
      var teacher = await _teacherFacade.GetByUserId(userId);

      var response = new TeacherStatusResponse
      {
        IsTeacher = teacher != null,
        Status = teacher?.Status,
        TeacherId = teacher?.Id,
        StatusMessage = GetStatusMessage(teacher?.Status)
      };

      return QueryResult(response);
    }

    private string GetStatusMessage(TeacherStatus? status)
    {
      if (status == null) return "Not a teacher";

      return status switch
      {
        TeacherStatus.Pending => "Your teacher application is pending approval",
        TeacherStatus.Active => "Active teacher",
        TeacherStatus.Inactive => "Your teacher account is inactive",
        _ => "Unknown status"
      };
    }
  }
}
