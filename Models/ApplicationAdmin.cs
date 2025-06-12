using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("tblApplicationAdmin")]
    public class ApplicationAdmin
    {
        [Key]
        [Column("ApplicationAdminId")]
        public Guid ApplicationAdminId { get; set; }

        [Required(ErrorMessage = "กรุณาเลือกแอปพลิเคชัน")]
        [Column("ApplicationId")]
        public Guid ApplicationId { get; set; } // ⭐ เปลี่ยนจาก string เป็น Guid

        [Required(ErrorMessage = "กรุณากรอกรหัสพนักงาน")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "ต้องมีความยาว 1-20 ตัวอักษร")]
        [Column("EmployeeNo")]
        public string EmployeeNo { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "ต้องมีความยาวไม่เกิน 100 ตัวอักษร")]
        [Column("FullName")]
        public string? FullName { get; set; } = string.Empty;

        [Column("CreateBy")]
        public string? CreateBy { get; set; }

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; }

        [Column("UpdateBy")]
        public string? UpdateBy { get; set; }

        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; }
    }
}