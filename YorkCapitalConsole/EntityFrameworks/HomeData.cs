namespace EntityFrameworks
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class HomeData : DbContext
    {
        public HomeData()
            : base("name=HomeData")
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .Property(e => e.Street)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Society)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Country)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.PrimaryPhone)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.OtherPhone)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Email)
                .IsUnicode(false);
        }
    }
}
