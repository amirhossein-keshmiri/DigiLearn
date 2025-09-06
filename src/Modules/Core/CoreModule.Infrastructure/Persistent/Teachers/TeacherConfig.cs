using CoreModule.Infrastructure.Persistent.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreModule.Infrastructure.Persistent.Teachers
{
  public class TeacherConfig : IEntityTypeConfiguration<Domain.Teachers.Models.Teacher>
  {
    public void Configure(EntityTypeBuilder<Domain.Teachers.Models.Teacher> builder)
    {
      builder.ToTable("Teachers");
      builder.HasKey(b => b.Id);
      builder.HasIndex(b => b.UserName).IsUnique();

      builder.Property(b => b.UserName)
          .IsRequired()
          .IsUnicode(false)
          .HasMaxLength(20);

      builder.HasOne<User>()
          .WithOne()
          .HasForeignKey<Domain.Teachers.Models.Teacher>(f => f.UserId);
    }
  }
}
