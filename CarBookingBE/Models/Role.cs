using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarBookingTest.Models
{
    [Table("RoleOfAccount")]
    public class Role
    {
        [Key]
        public int Id { get; set; }
        /*[Index(IsUnique = true)]
        [MaxLength(30)]*/
        public string Title { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public virtual ICollection<AccountRole> UserRoles { get; set; }
    }
}