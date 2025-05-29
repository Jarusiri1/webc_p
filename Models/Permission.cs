using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Permission
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();   // สร้าง GUID อัตโนมัติ

        [Required(ErrorMessage = "กรุณากรอกชื่อสิทธิ์")]
        [Display(Name = "ชื่อสิทธิ์")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "กรุณากรอกคำอธิบาย")]
        [Display(Name = "คำอธิบาย")]
        public string Description { get; set; } = default!;
    }
}
