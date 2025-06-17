using Microsoft.EntityFrameworkCore;
using TicketModule.Data.Entities;

namespace TicketModule.Data.Context
{
  class TicketContext : DbContext
  {
    public TicketContext(DbContextOptions<TicketContext> options) : base(options)
    {

    }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketMessage> TicketMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
      base.OnConfiguring(optionsBuilder);
    }
  }
}
