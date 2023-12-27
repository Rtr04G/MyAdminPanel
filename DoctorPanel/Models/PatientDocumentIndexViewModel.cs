namespace DoctorPanel.Models
{
    public class PatientDocumentIndexViewModel
    {
        public List<PatientDocument> Documents { get; set; }
        public string CurrentSort { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
