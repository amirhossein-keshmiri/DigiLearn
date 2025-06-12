using AutoMapper;
using BlogModule.Domain;
using BlogModule.Repositories.Categories;
using BlogModule.Repositories.Posts;
using BlogModule.Services.DTOs.Command;
using BlogModule.Services.DTOs.Query;
using Common.Application;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BlogModule.Services
{
  public interface IBlogService
  {
    Task<OperationResult> CreateBlogCategory(CreateBlogCategoryCommand command);
    Task<OperationResult> EditBlogCategory(EditBlogCategoryCommand command);
    Task<OperationResult> DeleteBlogCategory(Guid categoryId);
    Task<List<BlogCategoryDto>> GetAllBlogCategories();
    Task<BlogCategoryDto> GetBlogCategoryById(Guid categoryId);
  }

  class BlogService : IBlogService
  {
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    public BlogService(ICategoryRepository categoryRepository, IMapper mapper, IPostRepository postRepository)
    {
      _categoryRepository = categoryRepository;
      _mapper = mapper;
      _postRepository = postRepository;
    }

    public async Task<OperationResult> CreateBlogCategory(CreateBlogCategoryCommand command)
    {
      var category = _mapper.Map<Category>(command);

      if (await _categoryRepository.ExistsAsync(f => f.Slug == category.Slug))
      {
        return OperationResult.Error("Slug is Exist");
      }

      _categoryRepository.Add(category);
      await _categoryRepository.Save();
      return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteBlogCategory(Guid categoryId)
    {
      var category = await _categoryRepository.GetAsync(categoryId);
      if (category == null)
        return OperationResult.NotFound();

      if (await _postRepository.ExistsAsync(f => f.CategoryId == categoryId))
        return OperationResult.Error("این دسته بندی قبلا استفاده شده است ، لطفا پست های مربوطه را حذف کنید و دوباره امتحان کنید");

      _categoryRepository.DeleteCategory(category);
      await _categoryRepository.Save();

      return OperationResult.Success();
    }

    public async Task<OperationResult> EditBlogCategory(EditBlogCategoryCommand command)
    {
      var category = await _categoryRepository.GetAsync(command.Id);
      if (category == null)
        return OperationResult.NotFound();

      if (command.Slug != category.Slug)
      {
        if (await _categoryRepository.ExistsAsync(f => f.Slug == command.Slug))
          return OperationResult.Error("Slug is Exist");
      }

      category.Slug = command.Slug;
      category.Title = command.Title;

      _categoryRepository.Update(category);
      await _categoryRepository.Save();
      return OperationResult.Success();
    }


    public async Task<List<BlogCategoryDto>> GetAllBlogCategories()
    {
      var categories = await _categoryRepository.GetAllBlogCategories();
      return _mapper.Map<List<BlogCategoryDto>>(categories);
    }

    public async Task<BlogCategoryDto> GetBlogCategoryById(Guid categoryId)
    {
      var category = await _categoryRepository.GetAsync(categoryId);
      return _mapper.Map<BlogCategoryDto>(category);
    }
  }
}
