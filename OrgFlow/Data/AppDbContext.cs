using Microsoft.EntityFrameworkCore;
using OrgFlow.Entities;

namespace OrgFlow.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) //Dependency injection üzerinden bağlantı ayarlarınıı almak için constructor ekliyoruz
    : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e=>e.RegistrationNumber).HasMaxLength(11).IsRequired();
                entity.HasIndex(e=>e.RegistrationNumber).IsUnique();

                //SELF REFERENCİNG RELATİONSHİP:Entity'nin kendi tablosu  ile kurduğu ilişkiye self referencing denir
                //Employee tablosunun kendisi kendisiyle ilişkili
                //HasOne(): çalışanın en fazla bir yöneticisi olabilir
                //WithMany():Yöneticinin birden fazla astı olabilir
                //OnDelete():Bağlı astlar varken yöneticinin silinmesini veritabanı düzeyinde engeller
                entity.HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e=>e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict); 
                





            });
        }
    }
}
