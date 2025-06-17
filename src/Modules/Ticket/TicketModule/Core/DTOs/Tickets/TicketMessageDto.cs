namespace TicketModule.Core.DTOs.Tickets
{
  public class TicketMessageDto
  {
    public Guid UserId { get; set; }

    public string UserFullName { get; set; }

    public string Text { get; set; }

    public DateTime CreationDate { get; set; }
  }
}
