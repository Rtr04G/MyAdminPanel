﻿using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPanel.Models
{
    public class Record
    {
        public int Id { get; set; }
        public string DoctorId{ get; set; }
        public string PatientId { get; set; }
        public DateTime DateX { get; set;}
    }
}
