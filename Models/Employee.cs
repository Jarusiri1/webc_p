using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("tblEmployee")]
    public class Employee
    {
        [Key]
        public Guid EmployeeId { get; set; }

        [Required]
        public string EmployeeNo { get; set; } = string.Empty;

        public string? FullName { get; set; }
    }
}
