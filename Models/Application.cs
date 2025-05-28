using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสแอปพลิเคชัน")]
        [Display(Name = "ApplicationId")]
        public string ApplicationId { get; set; } = default!;

        [Required(ErrorMessage = "กรุณากรอกชื่อแอปพลิเคชัน")]
        [Display(Name = "ApplicationName")]
        public string ApplicationName { get; set; } = default!;

        [Required(ErrorMessage = "กรุณาระบุสถานะ")]
        [Display(Name = "Status")]
        public string Status { get; set; } = default!;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ติดต่อ")]
        [Display(Name = "ContactName")]
        public string ContactName { get; set; } = default!;

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทร")]
        [RegularExpression(@"^0(?:6|8|9)\d{8}$",
            ErrorMessage = "เบอร์โทรต้องเป็นมือถือไทย 10 หลัก เช่น 0812345678")]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; } = default!;

        [Display(Name = "สร้างเมื่อ")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "แก้ไขล่าสุด")]
        public DateTime UpdatedDate { get; set; }
    }
}
