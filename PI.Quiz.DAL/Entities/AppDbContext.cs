using Microsoft.EntityFrameworkCore;

namespace PI.Quiz.DAL.Entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<UmUser> Users { get; set; }

        public DbSet<UmResetPassword> ResetPasswords { get; set; }

        public DbSet<UmToken> Tokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UmUser>()
                .HasKey(t => t.Username);
            modelBuilder.Entity<UmResetPassword>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<UmToken>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<UmUser>().HasData(
                new UmUser()
                {
                    Username = "dev.awesome.th@gmail.com",
                    Password = "$2a$12$4J8uA7ec/xnaH2xRxLAr0.A0D8EX3mJbKdoR.nwaoWhFf3qJz0ERa", //=>1234
                    FirstName = "Thanatmet",
                    LastName = "Thanarattanan",
                    NickName = "Billy",
                    Email = "dev.awesome.th@gmail.com",
                    BirthDate = "1992-07-04",
                    PhoneNumber = "0904990500",
                    Activated = true,
                    Deleted = false,
                    Role = "SuperAdmin",
                    CreatedDate = DateTime.Now,
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }
}
