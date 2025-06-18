using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("tblUserGroup")]
    public class UserGroup
    {
        [Key]
        public Guid UserGroupId { get; set; }

        [Required(ErrorMessage = "กรุณาเลือกกลุ่ม")]
        public Guid GroupId { get; set; }
        
        [Required(ErrorMessage = "กรุณากรอกรหัสพนักงาน")]
        [StringLength(50, ErrorMessage = "รหัสพนักงานต้องไม่เกิน 50 ตัวอักษร")]
        public string EmployeeNo { get; set; } = string.Empty;
        
        public virtual Group? Group { get; set; }
    }
}