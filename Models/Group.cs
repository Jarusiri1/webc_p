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

        [Required]
        [StringLength(100)]
        public string GroupName { get; set; } = string.Empty;
    }
}