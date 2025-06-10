using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("tblApplicationAdmin")] // ชื่อตารางในฐานข้อมูล
    public class ApplicationAdmin
    {
        [Key]
        public Guid ApplicationAdminId { get; set; }

        [Required]
        public string ApplicationId { get; set; } = string.Empty;

        [Required]
        public string EmployeeNo { get; set; } = string.Empty;
    }
}
