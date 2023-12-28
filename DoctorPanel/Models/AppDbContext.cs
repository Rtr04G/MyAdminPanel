using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DoctorPanel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<AdminUser>
{
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<LPU> LPUs { get; set; }
    public DbSet<AdminUser> AdminUsers { get; set; }
    public DbSet<Record> Records { get; set; }
    public DbSet<PatientDocument> PatientDocuments { get; set; }
    public DbSet<Document> Documents { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

}