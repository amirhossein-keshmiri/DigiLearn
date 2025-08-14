using TicketModule.Data.Entities;

namespace TicketModule.Core.DTOs.Tickets
{
  public class TicketDto
  {
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string OwnerFullName { get; set; }

    public string PhoneNumber { get; set; }

    public string Title { get; set; }

    public string Text { get; set; }

    public TicketStatus Status { get; set; }

    public TicketPriority Priority { get; set; }

    public DateTime CreationDate { get; set; }
    public string PersianCreationDate { get; set; }

    public List<TicketMessageDto> Messages { get; set; }
  }
}
