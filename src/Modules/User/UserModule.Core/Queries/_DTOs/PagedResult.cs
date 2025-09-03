namespace UserModule.Core.Queries._DTOs
{
  public class PagedResult<T>
  {
    public List<T> Data { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public long TotalCount { get; set; }
    public int Take { get; set; }
  }
}
