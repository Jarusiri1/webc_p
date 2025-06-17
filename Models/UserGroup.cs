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

        [Required]
        public Guid GroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeNo { get; set; } = string.Empty;

        public Group? Group { get; set; }
    }
}