using Common.Application;
using TicketModule.Core.DTOs.Tickets;

namespace TicketModule.Core.Services
{
  public interface ITicketService
  {
    Task<OperationResult<Guid>> CreateTicket(CreateTicketCommand createTicketCommand);
    Task<OperationResult> SendMessageInTicket(SendTicketMessageCommand sendTicketMessageCommand);
    Task<OperationResult> CloseTicket(Guid ticketId);

    Task<TicketDto?> GetTicket(Guid ticketId);
    Task<TicketFilterResult> GetTicketsByFilter(TicketFilterParams filterParams);
  }
}
