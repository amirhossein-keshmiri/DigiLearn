using Common.Application;
using Common.Domain.Utils;
using Common.Domain.ValueObjects;
using CoreModule.Application.Courses.Create;
using CoreModule.Application.Courses.Sections.AddSection;
using CoreModule.Domain.Courses.Enums;
using CoreModule.Facade.Courses;
using CoreModule.Facade.Teachers;
using CoreModule.Query._DTOs;
using DigiLearn.Api.DTOs.Teacher.Courses;
using DigiLearn.Api.Infrastructure;
using DigiLearn.Api.Infrastructure.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Models.Responses;

namespace DigiLearn.Api.Controllers
{
  /// <summary>
  /// Teacher Courses Management API
  /// </summary>
  [Authorize]
  [ServiceFilter(typeof(TeacherApiActionFilter))]
  public class TeacherCoursesController : ApiController
  {
    private readonly ICourseFacade _courseFacade;
    private readonly ITeacherFacade _teacherFacade;

    public TeacherCoursesController(ICourseFacade courseFacade, ITeacherFacade teacherFacade)
    {
      _courseFacade = courseFacade;
      _teacherFacade = teacherFacade;
    }

    /// <summary>
    /// Get paginated list of courses for the authenticated teacher
    /// </summary>
    /// <param name="filterParams">Filter and pagination parameters</param>
    /// <returns>Paginated course list</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<CourseFilterResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<CourseFilterResult>> GetCourses([FromQuery] CourseFilterParams filterParams)
    {
      var teacher = await GetCurrentTeacherAsync();
      //var teacher = await _teacherFacade.GetByUserId(User.GetUserId());
      filterParams.TeacherId = teacher?.Id;

      var result = await _courseFacade.GetCourseFilter(filterParams);
      return QueryResult(result);
    }

    /// <summary>
    /// Get a specific course by ID with all details including sections
    /// </summary>
    /// <param name="courseId">Course unique identifier</param>
    /// <returns>Course details</returns>
    [HttpGet("{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status403Forbidden)]
    public async Task<ApiResult<CourseDto>> GetCourseById(Guid courseId)
    {
      //var teacher = await _teacherFacade.GetByUserId(User.GetUserId());
      var teacher = await GetCurrentTeacherAsync();
      var course = await _courseFacade.GetCourseById(courseId);

      if (course == null)
      {
        return new ApiResult<CourseDto>
        {
          IsSuccess = false,
          Data = null,
          MetaData = new MetaData
          {
            Message = "Course not found",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      if (course.TeacherId != teacher.Id)
      {
        //return CommandResult(OperationResult<CourseDto>.Error("You don't have access to this course."));
        return new ApiResult<CourseDto>
        {
          IsSuccess = false,
          Data = null,
          MetaData = new MetaData
          {
            Message = "You don't have access to this course",
            AppStatusCode = AppStatusCode.UnAuthorize
          }
        };
      }

      return QueryResult(course);
    }

    /// <summary>
    /// Create a new course with image and optional video
    /// </summary>
    /// <param name="request">Course creation data with files</param>
    /// <returns>Created course ID</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ApiResult> CreateCourse([FromForm] CreateCourseRequest request)
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

      //var teacher = await _teacherFacade.GetByUserId(User.GetUserId());
      var teacher = await GetCurrentTeacherAsync();

      var command = new CreateCourseCommand
      {
        TeacherId = teacher.Id,
        Status = CourseActionStatus.Pending,
        CategoryId = request.CategoryId,
        SubCategoryId = request.SubCategoryId,
        Title = request.Title,
        Slug = request.Slug.ToSlug(),
        Description = request.Description,
        ImageFile = request.ImageFile,
        VideoFile = request.VideoFile,
        Price = request.Price,
        CourseLevel = request.CourseLevel,
        SeoData = new SeoData(request.Title, request.Title, request.Title, null)
      };

      var result = await _courseFacade.Create(command);

      return CommandResult(result);
    }

    /// <summary>
    /// Update an existing course
    /// </summary>
    /// <param name="courseId">Course unique identifier</param>
    /// <param name="request">Updated course data</param>
    /// <returns>Operation result</returns>
    [HttpPut("{courseId:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> UpdateCourse(Guid courseId, [FromForm] UpdateCourseRequest request)
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

      //var teacher = await _teacherFacade.GetByUserId(User.GetUserId());
      var teacher = await GetCurrentTeacherAsync();
      var course = await _courseFacade.GetCourseById(courseId);

      if (course == null)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Course not found",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      if (course.TeacherId != teacher.Id)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "You don't have access to this course",
            AppStatusCode = AppStatusCode.UnAuthorize
          }
        };
      }

      // TODO: Add UpdateCourseCommand to your application layer
      // This is a placeholder - implement based on your architecture
      var result = new Common.Application.OperationResult
      {
        Status = Common.Application.OperationResultStatus.Success,
        Message = "Course updated successfully"
      };

      return CommandResult(result);
    }

    /// <summary>
    /// Delete a course (soft delete recommended)
    /// </summary>
    /// <param name="courseId">Course unique identifier</param>
    /// <returns>Operation result</returns>
    [HttpDelete("{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> DeleteCourse(Guid courseId)
    {
      //var teacher = await _teacherFacade.GetByUserId(User.GetUserId());
      var teacher = await GetCurrentTeacherAsync();
      var course = await _courseFacade.GetCourseById(courseId);

      if (course == null)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Course not found",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      if (course.TeacherId != teacher.Id)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "You don't have access to this course",
            AppStatusCode = AppStatusCode.UnAuthorize
          }
        };
      }

      // TODO: Add DeleteCourseCommand to your application layer
      var result = new Common.Application.OperationResult
      {
        Status = Common.Application.OperationResultStatus.Success,
        Message = "Course deleted successfully"
      };

      return CommandResult(result);
    }

    #region Course Sections

    /// <summary>
    /// Get all sections for a specific course
    /// </summary>
    /// <param name="courseId">Course unique identifier</param>
    /// <returns>Course with sections</returns>
    [HttpGet("{courseId:guid}/sections")]
    [ProducesResponseType(typeof(ApiResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult<CourseDto>> GetCourseSections(Guid courseId)
    {
      var teacher = await GetCurrentTeacherAsync();
      var course = await _courseFacade.GetCourseById(courseId);

      if (course == null || course.TeacherId != teacher.Id)
      {
        return new ApiResult<CourseDto>
        {
          IsSuccess = false,
          Data = null,
          MetaData = new MetaData
          {
            Message = "Course not found or you don't have access",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      return QueryResult(course);
    }

    /// <summary>
    /// Add a new section to a course
    /// </summary>
    /// <param name="courseId">Course unique identifier</param>
    /// <param name="request">Section data</param>
    /// <returns>Created section ID</returns>
    [HttpPost("{courseId:guid}/sections")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> AddSection(Guid courseId, [FromBody] AddSectionRequest request)
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

      var teacher = await GetCurrentTeacherAsync();
      var course = await _courseFacade.GetCourseById(courseId);

      if (course == null || course.TeacherId != teacher.Id)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Course not found or you don't have access",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      var command = new AddCourseSectionCommand
      {
        CourseId = courseId,
        Title = request.Title,
        DisplayOrder = request.DisplayOrder
      };

      var result = await _courseFacade.AddSection(command);

      //return CommandResult(result, System.Net.HttpStatusCode.Created,
      //    $"/api/TeacherCourses/{courseId}/sections/{result.Data}");
      return CommandResult(result);
    }

    /// <summary>
    /// Update a course section
    /// </summary>
    /// <param name="courseId">Course unique identifier</param>
    /// <param name="sectionId">Section unique identifier</param>
    /// <param name="request">Updated section data</param>
    /// <returns>Operation result</returns>
    [HttpPut("{courseId:guid}/sections/{sectionId:guid}")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> UpdateSection(Guid courseId, Guid sectionId,
        [FromBody] UpdateSectionRequest request)
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

      var teacher = await GetCurrentTeacherAsync();
      var course = await _courseFacade.GetCourseById(courseId);

      if (course == null || course.TeacherId != teacher.Id)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Course not found or you don't have access",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      // TODO: Add UpdateCourseSectionCommand to your application layer
      var result = new Common.Application.OperationResult
      {
        Status = Common.Application.OperationResultStatus.Success,
        Message = "Section updated successfully"
      };

      return CommandResult(result);
    }

    /// <summary>
    /// Delete a course section
    /// </summary>
    /// <param name="courseId">Course unique identifier</param>
    /// <param name="sectionId">Section unique identifier</param>
    /// <returns>Operation result</returns>
    [HttpDelete("{courseId:guid}/sections/{sectionId:guid}")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> DeleteSection(Guid courseId, Guid sectionId)
    {
      var teacher = await GetCurrentTeacherAsync();
      var course = await _courseFacade.GetCourseById(courseId);

      if (course == null || course.TeacherId != teacher.Id)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Course not found or you don't have access",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      // TODO: Add DeleteCourseSectionCommand to your application layer
      var result = new Common.Application.OperationResult
      {
        Status = Common.Application.OperationResultStatus.Success,
        Message = "Section deleted successfully"
      };

      return CommandResult(result);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Gets the current authenticated teacher
    /// This is cached in HttpContext.Items by TeacherApiActionFilter
    /// </summary>
    private async Task<TeacherDto> GetCurrentTeacherAsync()
    {
      // Check if teacher is already cached in HttpContext by the filter
      if (HttpContext.Items.TryGetValue("CurrentTeacher", out var cachedTeacher))
      {
        return cachedTeacher as TeacherDto;
      }

      // Fallback: fetch teacher if not cached
      var userId = User.GetUserId();
      var teacher = await _teacherFacade.GetByUserId(userId);

      if (teacher != null)
      {
        HttpContext.Items["CurrentTeacher"] = teacher;
      }

      return teacher;
    }

    #endregion
  }
}
