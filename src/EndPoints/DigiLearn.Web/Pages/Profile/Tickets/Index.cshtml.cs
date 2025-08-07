using Common.Application;
using Common.Application.DateUtil;
using DigiLearn.Web.Infrastructure;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TicketModule.Core.DTOs.Tickets;
using TicketModule.Core.Models.Request;
using TicketModule.Core.Services;
using TicketModule.Data.Entities;
using UserModule.Core.Services;

namespace DigiLearn.Web.Pages.Profile.Tickets
{
  [Authorize]
  public class IndexModel : BaseRazorFilter<TicketFilterParams>
  {
    private readonly ITicketService _ticketService;
    private readonly IUserFacade _userFacade;
    public IndexModel(ITicketService ticketService, IUserFacade userFacade)
    {
      _ticketService = ticketService;
      _userFacade = userFacade;
    }

    public TicketFilterResult FilterResult { get; set; }

    [BindProperty]
    public AddTicketRequest AddTicketRequest { get; set; }

    //public TicketDto Ticket { get; set; }

    [BindProperty(SupportsGet = true)]
    public TicketFilterParams? TicketFilterParams { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageId { get; set; } = 1;

    public async Task OnGet()
    {
      // Use PageId from query string
      var pageId = PageId < 1 ? 1 : PageId;

      FilterResult = await _ticketService.GetTicketsByFilter(new TicketFilterParams()
      {
        UserId = User.GetUserId(),
        Take = FilterParams.Take,
        PageId = FilterParams.PageId,
        Priority = TicketFilterParams?.Priority,
        Status = TicketFilterParams?.Status,
        Title = TicketFilterParams?.Title
      });
    }

    public class FilterResultDto
    {
      public List<TicketFilterData> Data { get; set; }
      public int CurrentPage { get; set; }
      public int TotalPages { get; set; }
      public long TotalCount { get; set; }
      public int Take { get; set; }
    }

    public async Task<IActionResult> OnGetFilter()
    {
      var filterParams = new TicketFilterParams
      {
        UserId = User.GetUserId(),
        Status = Enum.TryParse<TicketStatus>(Request.Query["TicketFilterParams.Status"], out var s) ? (TicketStatus?)s : null,
        Priority = Enum.TryParse<TicketPriority>(Request.Query["TicketFilterParams.Priority"], out var p) ? (TicketPriority?)p : null,
        Title = Request.Query["TicketFilterParams.Title"],
        PageId = int.TryParse(Request.Query["filterParams.pageId"], out var page) ? page : 1,
        Take = 10
      };

      var result = await _ticketService.GetTicketsByFilter(filterParams);

      var dto = new
      {
        success = true,
        filterResult = new
        {
          data = result.Data.Select(t => new
          {
            id = t.Id,
            title = t.Title,
            creationDate = t.CreationDate.ToPersianDate(),
            status = t.Status.ToString(),
            priority = t.Priority.ToString()
          }).ToList(),
          currentPage = result.CurrentPage,
          totalPages = result.PageCount,
          totalCount = result.EntityCount,
          take = result.Take
        }
      };

      return new JsonResult(dto);
    }

    public async Task<IActionResult> OnPostAddTicket()
    {
      var user = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());
      if (user == null)
      {
        return RedirectToPage("Index");
      }

      var sanitizer = new HtmlSanitizer();
      var sanitizedContent = sanitizer.Sanitize(AddTicketRequest.Text);

      var command = new CreateTicketCommand()
      {
        PhoneNumber = User.GetPhoneNumber(),
        OwnerFullName = $"{user.Name} {user.Family}",
        Text = sanitizedContent,
        Title = AddTicketRequest.Title,
        Priority = AddTicketRequest.Priority,
        UserId = user.Id
      };

      var result = await _ticketService.CreateTicket(command);
      return RedirectAndShowAlert(result, RedirectToPage("Index"));
    }

    public async Task<IActionResult> OnGetTicketById(Guid ticketId)
    {
      var ticket = await _ticketService.GetTicket(ticketId);
      if (ticket == null || ticket.UserId != User.GetUserId())
      {
        return new JsonResult(new { success = false });
      }

      // Map TicketDto to a simplified DTO for JSON serialization
      var ticketDetails = new
      {
        Id = ticket.Id,
        Title = ticket.Title,
        Text = ticket.Text,
        CreationDate = ticket.CreationDate.ToPersianDateTime(),
        Priority = ticket.Priority.ToString(),
        Status = ticket.Status.ToString(),
        Category = "Mailing Issues", // Replace with actual category logic
        Messages = ticket.Messages?
        .OrderBy(m => m.CreationDate)
        .Select(m => new
        {
          UserId = m.UserId,
          UserFullName = m.UserFullName,
          Text = m.Text,
          CreationDate = m.CreationDate.ToPersianDateTime()
        }).ToList()
      };

      return new JsonResult(new { success = true, ticket = ticketDetails });
    }

    //public async Task<IActionResult> OnGetTicketById()
    //{
    //  var ticket = await _ticketService.GetTicket(TicketId);
    //  if (ticket == null || ticket.UserId != User.GetUserId())
    //    //return RedirectToPage("Index");
    //    return new JsonResult(new { success = false });

    //  Ticket = ticket;
    //  //return Page();
    //  return new JsonResult(new { success = true, ticket });
    //}

    //public async Task<IActionResult> OnPostAddReply(Guid ticketId, string message)
    //{
    //  var user = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());
    //  var ticketMessage = new SendTicketMessageCommand()
    //  {
    //    TicketId = ticketId,
    //    UserId = User.GetUserId(),
    //    Text = message,
    //    OwnerFullName = $"{user.Name} {user.Family}"
    //  };

    //  var result = await _ticketService.SendMessageInTicket(ticketMessage);
    //  if (result.Status == OperationResultStatus.Success)
    //  {
    //    //return new JsonResult(new { success = true, message = result.Message });
    //    return new JsonResult(new
    //    {
    //      success = true,
    //      message = new
    //      {
    //        Text = result.Message,
    //        CreationDate = DateTime.Now.ToPersianDateTime(), // Or use server-created date
    //        UserFullName = ticketMessage.OwnerFullName
    //      }
    //    });
    //  }

    //  return new JsonResult(new { success = false });
    //  //return RedirectAndShowAlert(result, RedirectToPage("Index"));
    //  //return RedirectAndShowAlert(result, RedirectToPage("Index", new {ticketId}));
    //}

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostAddReply(Guid ticketId, string text)
    {
      var user = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());
      if (user == null || string.IsNullOrEmpty(text))
      {
        return new JsonResult(new { success = false, error = "Invalid input." });
      }

      var ticketMessage = new SendTicketMessageCommand()
      {
        TicketId = ticketId,
        UserId = User.GetUserId(),
        Text = text,
        OwnerFullName = $"{user.Name} {user.Family}"
      };

      try
      {
        var result = await _ticketService.SendMessageInTicket(ticketMessage);

        if (result.Status == OperationResultStatus.Success)
        {
          return new JsonResult(new
          {
            success = true,
            message = new
            {
              Text = text,
              UserId = ticketMessage.UserId,
              CreationDate = DateTime.Now.ToPersianDateTime(), // Or use server-created date
              UserFullName = ticketMessage.OwnerFullName
            }
          });
        }

        return new JsonResult(new { success = false, error = result.Message });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error sending message: {ex}");
        return new JsonResult(new { success = false, error = "Internal server error." });
      }
    }

    //public async Task<IActionResult> OnPostCloseTicket(Guid ticketId)
    //{
    //  return await AjaxTryCatch(async () =>
    //  {
    //    var ticket = await _ticketService.GetTicket(ticketId);

    //    if (ticket == null || ticket.UserId != User.GetUserId())
    //      return OperationResult.Error("Ticket Not Found!");

    //    return await _ticketService.CloseTicket(ticketId);
    //  });
    //}

    public async Task<IActionResult> OnPostCloseTicket(Guid ticketId)
    {
      var ticket = await _ticketService.GetTicket(ticketId);

      if (ticket == null || ticket.UserId != User.GetUserId())
        return new JsonResult(new { success = false, message = "Ticket not found or access denied." });

      if (ticket.Status == TicketStatus.Closed)
      {
        return new JsonResult(new { success = false, message = "Ticket is already closed." });
      }

      var result = await _ticketService.CloseTicket(ticketId);
      if (result.Status == OperationResultStatus.Success)
      {
        return new JsonResult(new { success = true });
      }

      return new JsonResult(new { success = false, message = result.Message });
    }

  }
}
