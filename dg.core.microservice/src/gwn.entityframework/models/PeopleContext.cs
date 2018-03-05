
using Microsoft.EntityFrameworkCore;


namespace gwn.entityframework.models
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
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                  .IsRequired()
                  .HasMaxLength(25);

                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.Property(e => e.ModifiedOn).HasDefaultValueSql("getdate()");

                entity.Property(e => e.ModifiedBy)
                 .IsRequired()
                 .HasMaxLength(100);

                //entity.Property(e => e.RegistrationDate).HasDefaultValueSql("getdate()");

                //entity.Property(e => e.Title)
                //    .IsRequired()
                //    .HasMaxLength(8);
            });
        }
    }
}