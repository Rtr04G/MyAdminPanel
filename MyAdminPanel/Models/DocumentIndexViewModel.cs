using MyAdminPanel.Models;
namespace MyAdminPanel.Models
{
    public class DocumentIndexViewModel
    {
        public List<Document> Documents { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
