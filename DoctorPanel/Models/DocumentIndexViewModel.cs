using DoctorPanel.Models;
namespace DoctorPanel.Models
{
    public class DocumentIndexViewModel
    {
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
