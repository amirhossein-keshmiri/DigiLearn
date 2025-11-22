using CoreModule.Application.Categories.Create;
using CoreModule.Application.Categories.Edit;
using CoreModule.Application.Categories.AddChild;
using CoreModule.Facade.Categories;
using CoreModule.Query._DTOs;
using DigiLearn.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Models.Responses;

namespace DigiLearn.Api.Controllers
{
  /// <summary>
  /// Course Categories Management API
  /// </summary>
  public class CourseCategoriesController : ApiController
  {
    private readonly ICourseCategoryFacade _categoryFacade;

    public CourseCategoriesController(ICourseCategoryFacade categoryFacade)
    {
      _categoryFacade = categoryFacade;
    }

    /// <summary>
    /// Get all main course categories (categories without parent)
    /// </summary>
    /// <returns>List of main categories</returns>
    [HttpGet("main")]
    [ProducesResponseType(typeof(ApiResult<List<CourseCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ApiResult<List<CourseCategoryDto>>> GetMainCategories()
    {
      var categories = await _categoryFacade.GetMainCategories();
      return QueryResult(categories);
    }

    /// <summary>
    /// Get all categories (including nested children)
    /// </summary>
    /// <returns>List of all categories with their children</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<List<CourseCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ApiResult<List<CourseCategoryDto>>> GetAllCategories()
    {
      var categories = await _categoryFacade.GetMainCategories();
      return QueryResult(categories);
    }

    /// <summary>
    /// Get a specific category by ID
    /// </summary>
    /// <param name="id">Category unique identifier</param>
    /// <returns>Category details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<CourseCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult<CourseCategoryDto>> GetCategoryById(Guid id)
    {
      var category = await _categoryFacade.GetById(id);

      if (category == null)
      {
        return new ApiResult<CourseCategoryDto>
        {
          IsSuccess = false,
          Data = null,
          MetaData = new MetaData
          {
            Message = "Category not found",
            AppStatusCode = AppStatusCode.NotFound
          }
        };
      }

      return QueryResult(category);
    }

    /// <summary>
    /// Get child categories for a specific parent category
    /// </summary>
    /// <param name="parentId">Parent category unique identifier</param>
    /// <returns>List of child categories</returns>
    [HttpGet("{parentId:guid}/children")]
    [ProducesResponseType(typeof(ApiResult<List<CourseCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ApiResult<List<CourseCategoryDto>>> GetChildCategories(Guid parentId)
    {
      var children = await _categoryFacade.GetChildren(parentId);
      return QueryResult(children);
    }

    /// <summary>
    /// Get subcategories by parent ID (alternative endpoint for compatibility)
    /// </summary>
    /// <param name="id">Parent category ID</param>
    /// <returns>List of subcategories</returns>
    //[HttpGet("subcategories/{id:guid}")]
    //[ProducesResponseType(typeof(ApiResult<List<CourseCategoryDto>>), StatusCodes.Status200OK)]
    //public async Task<ApiResult<List<CourseCategoryDto>>> GetSubCategories(Guid id)
    //{
    //  var children = await _categoryFacade.GetChildren(id);
    //  return QueryResult(children);
    //}

    #region Admin/Management Endpoints

    /// <summary>
    /// Create a new course category
    /// </summary>
    /// <param name="command">Category creation data</param>
    /// <returns>Operation result</returns>
    [HttpPost]
    //[Authorize(Roles = "Admin")] // Adjust role as needed
    [Authorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult> CreateCategory([FromBody] CreateCategoryCommand command)
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

      var result = await _categoryFacade.Create(command);
      return CommandResult(result);
    }

    /// <summary>
    /// Update an existing course category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="command">Updated category data</param>
    /// <returns>Operation result</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> UpdateCategory(Guid id, [FromBody] EditCategoryCommand command)
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

      // Ensure the ID in the route matches the command
      if (command.Id != id)
      {
        return new ApiResult
        {
          IsSuccess = false,
          MetaData = new MetaData
          {
            Message = "Category ID mismatch",
            AppStatusCode = AppStatusCode.BadRequest
          }
        };
      }

      var result = await _categoryFacade.Edit(command);
      return CommandResult(result);
    }

    /// <summary>
    /// Delete a course category
    /// </summary>
    /// <param name="id">Category ID to delete</param>
    /// <returns>Operation result</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> DeleteCategory(Guid id)
    {
      var result = await _categoryFacade.Delete(id);
      return CommandResult(result);
    }

    /// <summary>
    /// Add a child category to an existing parent category
    /// </summary>
    /// <param name="command">Child category data including parent ID</param>
    /// <returns>Operation result</returns>
    [HttpPost("add-child")]
    //[Authorize(Roles = "Admin")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ApiResult> AddChildCategory([FromBody] AddChildCategoryCommand command)
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

      var result = await _categoryFacade.AddChild(command);
      return CommandResult(result);
    }

    #endregion
  }
}