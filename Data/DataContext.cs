using HosterBackend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Data;

public class DataContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Authorize> Authorizes { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerType> CustomerTypes { get; set; }
    public DbSet<DomainProduct> DomainProducts { get; set; }
    public DbSet<DomainAccount> DomainAccounts { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<New> News { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RegisteredDomain> RegisteredDomains  { get; set; }
    public DataContext(DbContextOptions options) : base(options)
    {
        
    }
    protected DataContext(){

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Authorize>()
                    .HasOne(x => x.Employee)
                    .WithMany(x => x.HasRoles)
                    .HasForeignKey(x => x.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Authorize>()
                    .HasOne(x => x.Role)
                    .WithMany(x => x.GivenEmployees)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CustomerType>()
                    .HasOne(x => x.Discount)
                    .WithOne(x => x.CustomerType)
                    .HasForeignKey<Discount>(d => d.CustomerTypeId);
    }
}