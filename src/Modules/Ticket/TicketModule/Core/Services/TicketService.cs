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

      if (string.IsNullOrWhiteSpace(sendTicketMessageCommand.Text))
        return OperationResult.Error("Please Enter Ticket Text!");

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

    public async Task<TicketDto?> GetTicket(Guid ticketId)
    {
      var ticket = await _ticketContext.Tickets
                                       .Include(f => f.Messages)
                                       .FirstOrDefaultAsync(f => f.Id == ticketId);

      return _mapper.Map<TicketDto>(ticket);
    }

    public async Task<TicketFilterResult> GetTicketsByFilter(TicketFilterParams filterParams)
    {
      var result = _ticketContext.Tickets.OrderByDescending(x => x.CreationDate).AsQueryable();

      if (filterParams.UserId != null)
        result = result.Where(r => r.UserId == filterParams.UserId);

      if (string.IsNullOrWhiteSpace(filterParams.Title) == false)
        result = result.Where(r => r.Title.Contains(filterParams.Title));

      if (filterParams.Status != null)
        result = result.Where(r => r.Status == filterParams.Status);

      if (filterParams.Priority != null)
        result = result.Where(r => r.Priority == filterParams.Priority);

      var skip = (filterParams.PageId - 1) * filterParams.Take;
      var data = new TicketFilterResult()
      {
        Data = await result.Skip(skip).Take(filterParams.Take)
              .Select(n => new TicketFilterData
              {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Status = n.Status,
                Priority = n.Priority,
                CreationDate = n.CreationDate,
                OwnerFullName = n.OwnerFullName
              }).ToListAsync()
      };
      data.GeneratePaging(result, filterParams.Take, filterParams.PageId);
      return data;
    }
  }
}
