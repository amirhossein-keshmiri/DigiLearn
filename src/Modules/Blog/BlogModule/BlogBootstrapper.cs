﻿using BlogModule.Context;
using BlogModule.Repositories.Categories;
using BlogModule.Repositories.Posts;
using BlogModule.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogModule
{
  public static class BlogBootstrapper
  {
    public static IServiceCollection InitBlogModule(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<BlogContext>((context) =>
      {
        context.UseSqlServer(configuration.GetConnectionString("Blog_Context"));
      });
      services.AddScoped<ICategoryRepository, CategoryRepository>();
      services.AddScoped<IPostRepository, PostRepository>();

      services.AddScoped<IBlogService, BlogService>();
      services.AddAutoMapper(typeof(MapperProfile).Assembly);
      return services;
    }
  }
}