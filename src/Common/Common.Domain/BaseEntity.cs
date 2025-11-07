namespace Common.Domain
{
  public class BaseEntity
  {
    public Guid Id { get; set; }
    public DateTime CreationDate { get; set; }
    public BaseEntity()
    {
      //Id = Guid.NewGuid();
      Id = new Guid();
      CreationDate = DateTime.Now;
    }
  }
}
