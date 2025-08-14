namespace TicketModule.Core.Models.Response
{
  public class AddTicketReplyResponse
  {
    public string Text { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreateDateTime { get; set; }
    public string PersinaCreateDateTime { get; set; }
    public string UserFullName { get; set; }
  }
}
