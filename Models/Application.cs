using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Application
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "กรุณากรอก Application ID")]
        public string ApplicationId { get; set; } = default!;

        [Required(ErrorMessage = "กรุณากรอกชื่อแอปพลิเคชัน")]
        public string ApplicationName { get; set; } = default!;

        [Required(ErrorMessage = "กรุณาระบุสถานะ")]
        public string Status { get; set; } = default!;

        public string? Description { get; set; }

        public string? ContactName { get; set; }

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทร")]
        // รับเฉพาะมือถือไทย 10 หลัก (ขึ้นต้นด้วย 06, 08 หรือ 09)
        [RegularExpression(@"^0(?:6|8|9)\d{8}$", ErrorMessage = "เบอร์โทรต้องเป็นมือถือไทย 10 หลัก เช่น 0812345678")]
        public string Telephone { get; set; } = default!;
    }
}
