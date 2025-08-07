using System.ComponentModel.DataAnnotations;
using TicketModule.Data.Entities;

namespace TicketModule.Core.Models.Request
{
  public class AddTicketRequest
  {
    [Display(Name = "Title")]
    [Required(ErrorMessage = "Please Enter {0}")]
    public string Title { get; set; }

    [Display(Name = "Text")]
    [Required(ErrorMessage = "Please Enter {0}")]
    public string Text { get; set; }

    [Display(Name = "Priority")]
    [Required(ErrorMessage = "Please Enter {0}")]
    public TicketPriority Priority { get; set; }
  }
}
