using Microsoft.AspNetCore.Identity;

namespace PatientPanel.Models
{
    public class AdminUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string? MiddleName { get; set; }
        public string? Phone { get; set; }
        public string? Specialization { get; set; }
        public string? Category { get; set; }
        public string? Address { get; set; }
    }
}
