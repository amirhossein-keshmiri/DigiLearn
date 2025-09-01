namespace UserModule.Core.Queries._DTOs
{
  public class NotificationDto
  {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public bool IsSeen { get; set; }
    public DateTime CreationDate { get; set; }
    public string PersianCreationDate { get; set; }
  }
}
