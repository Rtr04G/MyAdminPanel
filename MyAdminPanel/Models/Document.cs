using System;
using System.ComponentModel.DataAnnotations;

namespace MyAdminPanel.Models
{

    public class Document
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; }

    }
}
