using BlogModule.Context;
using BlogModule.Domain;
using Common.Domain.Repository;
using Common.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace BlogModule.Repositories.Categories
{
  interface ICategoryRepository : IBaseRepository<Category>
  {
    void DeleteCategory(Category category);
    Task<List<Category>> GetAllBlogCategories();
  }

  class CategoryRepository : BaseRepository<Category, BlogContext>, ICategoryRepository
  {
    public CategoryRepository(BlogContext context) : base(context)
    {
    }

    public void DeleteCategory(Category category)
    {
      Context.Categories.Remove(category);
    }

    public async Task<List<Category>> GetAllBlogCategories()
    {
      return await Context.Categories.ToListAsync();
    }
  }
}
