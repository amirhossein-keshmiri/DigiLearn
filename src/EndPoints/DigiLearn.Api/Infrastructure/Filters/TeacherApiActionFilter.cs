using CoreModule.Domain.Teachers.Enums;
using CoreModule.Facade.Teachers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DigiLearn.Api.Infrastructure.Filters
{
  /// <summary>
  /// Action filter for API endpoints that require an active teacher account
  /// Validates that the authenticated user has an active teacher profile
  /// </summary>
  public class TeacherApiActionFilter : IAsyncActionFilter
  {
    private readonly ITeacherFacade _teacherFacade;
    private readonly ILogger<TeacherApiActionFilter> _logger;

    public TeacherApiActionFilter(ITeacherFacade teacherFacade, ILogger<TeacherApiActionFilter> logger)
    {
      _teacherFacade = teacherFacade;
      _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      // Check if user is authenticated
      if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
      {
        _logger.LogWarning("Unauthenticated user attempted to access teacher endpoint");

        context.Result = new JsonResult(new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Authentication required. Please login to continue.",
            AppStatusCode = AppStatusCode.UnAuthorize
          }
        })
        {
          StatusCode = StatusCodes.Status401Unauthorized
        };

        return;
      }

      // Get user ID from claims
      var userId = context.HttpContext.User.GetUserId();

      if (userId == Guid.Empty)
      {
        _logger.LogWarning("Failed to extract user ID from authenticated user");

        context.Result = new JsonResult(new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Invalid authentication token. Please login again.",
            AppStatusCode = AppStatusCode.UnAuthorize
          }
        })
        {
          StatusCode = StatusCodes.Status401Unauthorized
        };
        return;
      }

      // Fetch teacher profile
      var teacher = await _teacherFacade.GetByUserId(userId);

      // Check if teacher profile exists
      if (teacher == null)
      {
        _logger.LogInformation("User {UserId} attempted to access teacher endpoint without teacher profile", userId);

        context.Result = new JsonResult(new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Teacher profile not found. Please complete your teacher registration.",
            AppStatusCode = AppStatusCode.NotFound
          }
        })
        {
          StatusCode = StatusCodes.Status403Forbidden
        };
        return;
      }

      // Check if teacher is active
      if (teacher.Status != TeacherStatus.Active)
      {
        _logger.LogInformation("Inactive teacher {TeacherId} attempted to access teacher endpoint. Status: {Status}",
            teacher.Id, teacher.Status);

        var message = teacher.Status switch
        {
          TeacherStatus.Pending => "Your teacher profile is pending approval. Please wait for admin review.",
          //TeacherStatus.Rejected => "Your teacher profile has been rejected. Please contact support for more information.",
          _ => "Your teacher profile is not active. Please contact support."
        };

        context.Result = new JsonResult(new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = message,
            AppStatusCode = AppStatusCode.UnAuthorize
          }
        })
        {
          StatusCode = StatusCodes.Status403Forbidden
        };
        return;
      }

      // Cache teacher in HttpContext.Items for controller use
      context.HttpContext.Items["CurrentTeacher"] = teacher;

      // Continue to action
      await next();
    }
  }
}
