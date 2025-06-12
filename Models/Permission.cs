using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("tblPermission")] // ← ชื่อตารางใน SQL Server
    public class Permission
    {
        [Key]
        [Column("PermissionId")] // ← Primary key ชื่อจริงใน SQL
        public Guid PermissionId { get; set; }

        [Required(ErrorMessage = "กรุณากรอก Application ID")]
        [Column("ApplicationId")]
        public Guid ApplicationId { get; set; } = default!;

        [Required(ErrorMessage = "กรุณากรอกชื่อสิทธิ์")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "ต้องมีความยาว 2-100 ตัวอักษร")]
        public string PermissionName { get; set; } = string.Empty;

        // ⭐ แก้ไขส่วนนี้ - ลบ Required และ MinimumLength ออก
        [StringLength(200, ErrorMessage = "ต้องมีความยาวไม่เกิน 200 ตัวอักษร")]
        public string? Description { get; set; } = string.Empty;

        [Column("CreateBy")]
        public string? CreateBy { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        // ⭐ เพิ่ม properties เหล่านี้
        [Column("UpdateBy")]
        public string? UpdateBy { get; set; }

        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; }
    }
}