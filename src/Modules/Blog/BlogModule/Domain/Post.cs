using Common.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogModule.Domain
{
  [Index("Slug", IsUnique = true)]
  [Table("Posts", Schema = "dbo")]
  class Post : BaseEntity
  {
    [MaxLength(100)]
    public string Title { get; set; }

    public Guid UserId { get; set; }

    [MaxLength(100)]
    public string OwnerName { get; set; }

    public string Description { get; set; }

    [MaxLength(80)]
    public string Slug { get; set; }

    public long Visit { get; set; }

    [MaxLength(150)]
    public string ImageName { get; set; }

    public Guid CategoryId { get; set; }
  }
}
