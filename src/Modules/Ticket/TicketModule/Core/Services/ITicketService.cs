using Common.Application;
using TicketModule.Core.DTOs.Tickets;
using TicketModule.Core.Models.Response;

namespace TicketModule.Core.Services
{
  public interface ITicketService
  {
    Task<OperationResult<Guid>> CreateTicket(CreateTicketCommand createTicketCommand);
    Task<OperationResult<AddTicketReplyResponse>> SendMessageInTicket(SendTicketMessageCommand sendTicketMessageCommand);
    Task<OperationResult> CloseTicket(Guid ticketId);

    Task<TicketDto?> GetTicket(Guid ticketId);
    Task<TicketFilterResult> GetTicketsByFilter(TicketFilterParams filterParams);
  }
}
