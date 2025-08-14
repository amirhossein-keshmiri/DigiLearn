using ClosedXML.Excel;
using Common.Application;
using Common.Application.DateUtil;
using DigiLearn.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketModule.Core.DTOs.Tickets;
using TicketModule.Core.Models.Request;
using TicketModule.Core.Models.Response;
using TicketModule.Core.Services;
using TicketModule.Data.Entities;
using UserModule.Core.Services;

namespace DigiLearn.Api.Controllers
{
  [Authorize]
  public class TicketsController : ApiController
  {
    private readonly IUserFacade _userFacade;
    private readonly ITicketService _ticketService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TicketsController(IUserFacade userFacade, ITicketService ticketService, IHttpContextAccessor httpContextAccessor)
    {
      _userFacade = userFacade;
      _ticketService = ticketService;
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("GetTicketsByFilter")]
    public async Task<ApiResult<PagedResult<TicketFilterData>>> GetTicketsByFilter(
        [FromQuery] TicketStatus? status,
        [FromQuery] TicketPriority? priority,
        [FromQuery] string? title,
        [FromQuery] int page = 1,
        [FromQuery] int take = 10)
    {
      // Validate and parse enums
      var statusEnum = Enum.TryParse<TicketStatus>(status.ToString(), out var s) ? (TicketStatus?)s : null;
      var priorityEnum = Enum.TryParse<TicketPriority>(priority.ToString(), out var p) ? (TicketPriority?)p : null;

      var filterParams = new TicketFilterParams
      {
        UserId = User.GetUserId(),
        Status = statusEnum,
        Priority = priorityEnum,
        Title = title,
        PageId = page < 1 ? 1 : page,
        Take = take < 1 ? 10 : take
      };

      var result = await _ticketService.GetTicketsByFilter(filterParams);

      var dto = new PagedResult<TicketFilterData>
      {
        Data = result.Data.Select(t => new TicketFilterData
        {
          Id = t.Id,
          UserId = t.UserId,
          Title = t.Title,
          OwnerFullName = t.OwnerFullName,
          PersianCreationDate = t.CreationDate.ToPersianDate(),
          CreationDate = t.CreationDate,
          Status = t.Status,
          Priority = t.Priority
        }).ToList(),
        CurrentPage = result.CurrentPage,
        TotalPages = result.PageCount,
        TotalCount = result.EntityCount,
        Take = result.Take
      };

      return QueryResult(dto);
    }

    [HttpGet("GetTicket/{id:guid}")]
    public async Task<ApiResult<TicketDto>> GetTicket(Guid id)
    {
      var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
      if (userId == null)
      {
        return QueryResult(new TicketDto());
      }

      var ticket = await _ticketService.GetTicket(id);

      if (ticket == null || ticket.UserId != userId)
      {
        return QueryResult(new TicketDto());
      }

      var dto = new TicketDto
      {
        Id = ticket.Id,
        Title = ticket.Title,
        UserId = ticket.UserId,
        Text = ticket.Text,
        OwnerFullName = ticket.OwnerFullName,
        PhoneNumber = ticket.PhoneNumber,
        PersianCreationDate = ticket.CreationDate.ToPersianDateTime(),
        CreationDate = ticket.CreationDate,
        Status = ticket.Status,
        Priority = ticket.Priority,
        Messages = ticket.Messages?
                         .OrderBy(m => m.CreationDate)
                         .Select(m => new TicketMessageDto
                         {
                           UserId = m.UserId,
                           UserFullName = m.UserFullName,
                           Text = m.Text,
                           CreationDate = m.CreationDate,
                           PersianCreationDate = m.CreationDate.ToPersianDateTime()
                         }).ToList() ?? new List<TicketMessageDto>()
      };

      return QueryResult(dto);
    }

    [HttpPost("AddTicket")]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(401)]
    //[SwaggerOperation("Create a new support ticket")]
    //[SwaggerRequestExample(typeof(AddTicketRequest), typeof(AddTicketRequestExample))]
    public async Task<ApiResult<Guid>> AddTicket([FromBody] AddTicketRequest addTicketRequest)
    {
      // Validate input
      if (string.IsNullOrWhiteSpace(addTicketRequest.Title) ||
          string.IsNullOrWhiteSpace(addTicketRequest.Text))
      {
        var result = OperationResult<Guid>.Error("Title, Description, and Priority are required!");
        return CommandResult(result);
      }

      // Parse Priority
      if (!Enum.TryParse<TicketPriority>(addTicketRequest.Priority.ToString(), true, out var priority))
      {
        var result = OperationResult<Guid>.Error("Invalid priority value!");
        return CommandResult(result);
      }

      var user = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());

      // Get current user ID
      var userId = User.GetUserId();
      if (user == null)
      {
        var result = OperationResult<Guid>.Error("User Not Found!");
        return CommandResult(result);
      }

      // Map to command
      var command = new CreateTicketCommand
      {
        PhoneNumber = User.GetPhoneNumber(),
        UserId = user.Id,
        Title = addTicketRequest.Title,
        Text = addTicketRequest.Text,
        Priority = addTicketRequest.Priority,
        OwnerFullName = $"{user.Name} {user.Family}",
      };

      var sendTicketResult = await _ticketService.CreateTicket(command);
      return CommandResult(sendTicketResult);
    }

    [HttpPost("{ticketId:guid}/reply")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ApiResult<AddTicketReplyResponse?>> AddReply(Guid ticketId, [FromBody] string replay)
    {
      // Validate input
      if (string.IsNullOrWhiteSpace(replay))
      {
        var result = OperationResult<AddTicketReplyResponse>.Error("Title, Description, and Priority are required!");
        return CommandResult(result);
      }

      var user = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());
      if (user == null)
      {
        var result = OperationResult<AddTicketReplyResponse>.Error("User Not Found!");
        return CommandResult(result);
      }

      // Call service
      var command = new SendTicketMessageCommand
      {
        TicketId = ticketId,
        UserId = user.Id,
        Text = replay,
        OwnerFullName = $"{user.Name} {user.Family}"
      };

      var replyResult = await _ticketService.SendMessageInTicket(command);

      if (replyResult.Status != OperationResultStatus.Success)
      {
        var result = OperationResult<AddTicketReplyResponse>.Error("Operation Failed!");
        return CommandResult(result);
      }

      return CommandResult(replyResult);
    }

    [HttpPost("{ticketId:guid}/close")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ApiResult> CloseTicket(Guid ticketId)
    {
      var ticket = await _ticketService.GetTicket(ticketId);
      if (ticket == null || ticket.UserId != User.GetUserId())
      {
        var result = OperationResult.Error("Ticket not found or access denied!");
        return CommandResult(result);
      }

      if (ticket.Status == TicketStatus.Closed)
      {
        var result = OperationResult.Error("Ticket is already closed.");
        return CommandResult(result);
      }

      var closeTicketResult = await _ticketService.CloseTicket(ticketId);

      if (closeTicketResult.Status != OperationResultStatus.Success)
      {
        var result = OperationResult.Error(closeTicketResult.Message);
        return CommandResult(result);
      }

      return CommandResult(closeTicketResult);
    }

    [HttpGet("export")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    //[SwaggerOperation("Export tickets to Excel")]
    public async Task<IActionResult> ExportToExcel(
        [FromQuery] TicketStatus? status,
        [FromQuery] TicketPriority? priority,
        [FromQuery] string? title,
        [FromQuery] int page = 1,
        [FromQuery] int take = 10,
        [FromQuery] string exportType = "current") // "current" or "all"
    {
      // Get current user ID
      var userId = User.GetUserId();

      // Parse enums
      var statusEnum = Enum.TryParse<TicketStatus>(status.ToString(), true, out var s) ? (TicketStatus?)s : null;
      var priorityEnum = Enum.TryParse<TicketPriority>(priority.ToString(), true, out var p) ? (TicketPriority?)p : null;

      var filterParams = new TicketFilterParams
      {
        UserId = userId,
        Status = statusEnum,
        Priority = priorityEnum,
        Title = title,
        PageId = page < 1 ? 1 : page,
        Take = exportType == "all" ? int.MaxValue : (take < 1 ? 10 : take)
      };

      var result = await _ticketService.GetTicketsByFilter(filterParams);

      // Map to export data
      var exportData = result.Data.Select(t => new
      {
        TicketID = t.Id.ToString().ToUpper().Substring(0, 8),
        Subject = t.Title,
        Priority = t.Priority.ToString(),
        Status = t.Status switch
        {
          TicketStatus.Pending => "Pending",
          TicketStatus.Answered => "Answered",
          TicketStatus.Closed => "Closed",
          _ => "Unknown"
        },
        CreationDate = t.CreationDate.ToPersianDateTime()
      }).ToList();

      // Generate Excel file
      using var workbook = new XLWorkbook();
      var worksheet = workbook.Worksheets.Add("Tickets");
      worksheet.Cell(1, 1).InsertTable(exportData, "TicketsTable", true);
      worksheet.Columns().AdjustToContents();

      // Format header row
      var headerRow = worksheet.Row(1);
      headerRow.Style.Font.Bold = true;
      headerRow.Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);

      // Save to memory stream
      using var stream = new MemoryStream();
      workbook.SaveAs(stream);
      //stream.Position = 0;

      // ✅ Convert to byte array
      var fileBytes = stream.ToArray();

      // Generate filename
      var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
      var fileName = $"Tickets_{timestamp}.xlsx";

      return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
  }
}
