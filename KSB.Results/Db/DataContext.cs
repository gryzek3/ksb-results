using Microsoft.EntityFrameworkCore;

namespace KSB.Results.Db
{
    public class StartResult
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? Points { get; set; }
        public int? TensCount { get; set; }
        public double? Factor { get; set; }
        public double? Time { get; set; }
        public string Course { get; set; }
        public string Player { get; set; }
    }

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<StartResult> StartResults { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StartResult>(x =>
            {
                x.HasIndex(x => x.TimeStamp).IsDescending();
            });
        }
    }
}