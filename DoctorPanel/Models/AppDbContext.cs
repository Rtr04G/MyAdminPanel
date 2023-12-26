using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DoctorPanel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<AdminUser>
{
    public DbSet<AdminUser> AdminUsers { get; set; }
    public DbSet<Record> Records { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

}