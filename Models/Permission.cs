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

[Required(ErrorMessage = "กรุณากรอกคำอธิบาย")]
[StringLength(200, MinimumLength = 2, ErrorMessage = "ต้องมีความยาว 2-200 ตัวอักษร")]
public string Description { get; set; } = string.Empty;


        [Column("CreateBy")]
        public string? CreateBy { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

    }
}
