using AutoMapper;
using Common.Application;
using Common.Application.SecurityUtil;
using Microsoft.EntityFrameworkCore;
using TicketModule.Core.DTOs.Tickets;
using TicketModule.Data.Context;
using TicketModule.Data.Entities;

namespace TicketModule.Core.Services
{
  class TicketService : ITicketService
  {
    private readonly TicketContext _ticketContext;
    private readonly IMapper _mapper;
    public TicketService(TicketContext ticketContext, IMapper mapper)
    {
      _ticketContext = ticketContext;
      _mapper = mapper;
    }

    public async Task<OperationResult<Guid>> CreateTicket(CreateTicketCommand createTicketCommand)
    {
      var ticket = _mapper.Map<Ticket>(createTicketCommand);
      _ticketContext.Tickets.Add(ticket);
      await _ticketContext.SaveChangesAsync();

      return OperationResult<Guid>.Success(ticket.Id);
    }
    public async Task<OperationResult> SendMessageInTicket(SendTicketMessageCommand sendTicketMessageCommand)
    {
      var ticket = await _ticketContext.Tickets.FirstOrDefaultAsync(f => f.Id == sendTicketMessageCommand.TicketId);
      if (ticket == null)
      {
        return OperationResult.NotFound();
      }

      var message = new TicketMessage()
      {
        TicketId = ticket.Id,
        UserFullName = sendTicketMessageCommand.OwnerFullName,
        Text = sendTicketMessageCommand.Text.SanitizeText(),
        UserId = sendTicketMessageCommand.UserId,
      };

      if (ticket.UserId == sendTicketMessageCommand.UserId)
      {
        ticket.Status = TicketStatus.Pending;
      }
      else
      {
        ticket.Status = TicketStatus.Answered;
      }

      _ticketContext.TicketMessages.Add(message);
      _ticketContext.Tickets.Update(ticket);
      await _ticketContext.SaveChangesAsync();

      return OperationResult.Success();
    }

    public async Task<OperationResult> CloseTicket(Guid ticketId)
    {
      var ticket = await _ticketContext.Tickets.FirstOrDefaultAsync(f => f.Id == ticketId);
      if (ticket == null)
      {
        return OperationResult.NotFound();
      }

      ticket.Status = TicketStatus.Closed;

      _ticketContext.Tickets.Update(ticket);
      await _ticketContext.SaveChangesAsync();

      return OperationResult.Success();
    }

    public async Task<TicketDto> GetTicket(Guid ticketId)
    {
      var ticket = await _ticketContext.Tickets
                                       .Include(f => f.Messages)
                                       .FirstOrDefaultAsync(f => f.Id == ticketId);

      return _mapper.Map<TicketDto>(ticket);
    }
  }
}
