using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyAdminPanel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<AdminUser>
{
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<LPU> LPUs { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<AdminUser> AdminUsers { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

}