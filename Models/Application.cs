using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("tblApplication")]
    public class Application
    {
        [Key]
        [Column("ApplicationId")]
        public Guid ApplicationId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อแอปพลิเคชัน")]
        [Column("ApplicationName")]
        public string ApplicationName { get; set; } = default!;

        [Required(ErrorMessage = "กรุณาระบุสถานะ")]
        [Column("ApplicationStatus")]
        public string ApplicationStatus { get; set; } = default!;

        [Column("Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ติดต่อ")]
        [Column("ContactName")]
        public string ContactName { get; set; } = default!;

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทร")]
        [RegularExpression(@"^0(?:6|8|9)\d{8}$", ErrorMessage = "เบอร์โทรต้องเป็นมือถือไทย 10 หลัก เช่น 0812345678")]
        [Column("PhoneNumber")]
        public string Telephone { get; set; } = default!;

        [Column("CreateBy")]
        public string? CreateBy { get; set; }

        [Column("CreateDate")]
        public DateTime CreatedDate { get; set; }

        [Column("UpdateBy")]
        public string? UpdateBy { get; set; }

        [Column("UpdateDate")]
        public DateTime UpdatedDate { get; set; }
    }
}
