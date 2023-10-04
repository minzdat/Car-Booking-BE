using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CarBookingTest.Models
{
    [Table("AccountRole")]
    public class AccountRole
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public Account User { get; set; }
        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        [JsonIgnore]
        public Role Role { get; set; }
    }
}