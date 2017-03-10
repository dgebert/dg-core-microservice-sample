using Microsoft.EntityFrameworkCore;

namespace dg.repository.Models
{
    public partial class PeopleContext : DbContext
    {
        public virtual DbSet<Person> Person { get; set; }

        public PeopleContext(DbContextOptions<PeopleContext> options) : base(options) { }
   
        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BirthDate).HasDefaultValueSql("getdate()");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255);

                //entity.Property(e => e.RegistrationDate).HasDefaultValueSql("getdate()");

                //entity.Property(e => e.Title)
                //    .IsRequired()
                //    .HasMaxLength(8);
            });
        }
    }
}