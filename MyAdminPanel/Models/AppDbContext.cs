using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyAdminPanel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<AdminUser> AdminUsers { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
}