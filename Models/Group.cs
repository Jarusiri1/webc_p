using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("tblGroup")]
    public class Group
    {
        [Key]
        public Guid GroupId { get; set; }

        [Required(ErrorMessage = "กรุณาเลือกแอปพลิเคชัน")]
        public Guid ApplicationId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อกลุ่ม")]
        [StringLength(100, ErrorMessage = "ชื่อกลุ่มต้องไม่เกิน 100 ตัวอักษร")]
        [Display(Name = "ชื่อกลุ่ม")]
        public string GroupName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "คำอธิบายต้องไม่เกิน 500 ตัวอักษร")]
        [Display(Name = "คำอธิบาย")]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        [StringLength(50)]
        public string? UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        // Navigation property
        [ForeignKey("ApplicationId")]
        public virtual Application? Application { get; set; }
    }
}