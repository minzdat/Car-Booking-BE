using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarBookingTest.Models
{
    [Table("DepartmentMember")]
    public class DepartmentMember
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Position { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public Account User { get; set; }
        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }
}