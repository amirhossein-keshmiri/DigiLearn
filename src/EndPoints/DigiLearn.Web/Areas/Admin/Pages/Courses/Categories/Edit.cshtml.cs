using CoreModule.Application.Categories.Edit;
using CoreModule.Facade.Categories;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.Areas.Admin.Pages.Courses.Categories
{
    [BindProperties]
    public class EditModel : BaseRazor
    {
        private ICourseCategoryFacade _categoryFacade;

        public EditModel(ICourseCategoryFacade categoryFacade)
        {
            _categoryFacade = categoryFacade;
        }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please Enter {0}")]
        public string Title { get; set; }

        [Display(Name = "Slug")]
        [Required(ErrorMessage = "Please Enter {0}")]
        public string Slug { get; set; }
        public async Task<IActionResult> OnGet(Guid id)
        {
            var category = await _categoryFacade.GetById(id);
            if (category == null)
                return RedirectToPage("Index");

            Title = category.Title;
            Slug = category.Slug;
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid id)
        {
            var result = await _categoryFacade.Edit(new EditCategoryCommand()
            {
                Slug = Slug,
                Title = Title,
                Id = id
            });
            return RedirectAndShowAlert(result, RedirectToPage("Index"));
        }
    }
}
