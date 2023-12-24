using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyAdminPanel.Models;
public class AppDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
}