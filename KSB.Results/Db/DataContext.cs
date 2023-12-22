using Microsoft.EntityFrameworkCore;

namespace KSB.Results.Db
{
    public class StartResult
    {
        public int Id { get; set; }
        public int? Points { get; set; }
        public int? TensCount { get; set; }
        public double? Factor { get; set; }
        public double? Time { get; set; }
        public DateTime TimeStamp { get; set; }
        public Course Course { get; set; }
        public Player Player { get; set; }
    }
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<StartResult> StartResults { get; set; }
    }
    public class Player
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surnamename { get; set; }
        public string Name { get; set; }
        public string SpreadSheetName { get; set; }
        public Club Club { get; set; }
        public string LicenseNumber { get; set; }
        public List<StartResult> StartResults { get; set; }
    }
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StartResult> StartResults { get; set; }
    }
}