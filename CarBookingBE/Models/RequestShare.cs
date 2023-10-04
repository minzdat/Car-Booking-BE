using CarBookingTest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("RequestShare")]
    public class RequestShare
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public Account User { get; set; }
        public Guid? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }
    }
}