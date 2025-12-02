using Common.Domain.Utils;
using CoreModule.Application.Categories.AddChild;
using CoreModule.Application.Categories.Create;
using CoreModule.Facade.Categories;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.Areas.Admin.Pages.Courses.Categories
{
  [BindProperties]
  public class AddModel : BaseRazor
  {
    private ICourseCategoryFacade _categoryFacade;

    public AddModel(ICourseCategoryFacade categoryFacade)
    {
      _categoryFacade = categoryFacade;
    }

    [Display(Name = "Title")]
    [Required(ErrorMessage = "Please Enter {0}")]
    public string Title { get; set; }

    [Display(Name = "Slug")]
    [Required(ErrorMessage = "Please Enter {0}")]
    public string Slug { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost([FromQuery] Guid? parentId)
    {
      if (parentId != null)
      {
        var result = await _categoryFacade.AddChild(new AddChildCategoryCommand()
        {
          Title = Title,
          Slug = Slug.ToSlug(),
          ParentCategoryId = (Guid)parentId
        });

        return RedirectAndShowAlert(result, RedirectToPage("Index"));
      }
      else
      {
        var result = await _categoryFacade.Create(new CreateCategoryCommand
        {
          Title = Title,
          Slug = Slug.ToSlug()
        });

        return RedirectAndShowAlert(result, RedirectToPage("Index"));
      }

    }

  }
}
