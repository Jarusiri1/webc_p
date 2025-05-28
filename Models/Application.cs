using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyWebApp.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }              // PK แบบ auto-increment

        [Required]
        public string ApplicationId { get; set; } = default!; // business code, user input
        
        [Required(ErrorMessage = "กรุณากรอกชื่อแอปพลิเคชัน")]
        public string ApplicationName { get; set; } = default!;

        [Required(ErrorMessage = "กรุณาระบุสถานะ")]
        public string Status { get; set; } = default!;

        public string? Description { get; set; }

        public string? ContactName { get; set; }

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทร")]
        //ขึ้นต้นด้วย 06, 08 หรือ 09
        [RegularExpression(@"^0(?:6|8|9)\d{8}$", ErrorMessage = "เบอร์โทรต้องเป็นมือถือไทย 10 หลัก เช่น 0812345678")]
        public string Telephone { get; set; } = default!;

        [Display(Name = "สร้างเมื่อ")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "แก้ไขล่าสุด")]
        public DateTime UpdatedDate { get; set; }
    }
}