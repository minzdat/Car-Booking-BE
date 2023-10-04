using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingBE.Models
{
    [Table("Rotation")]
    public class Rotation
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public virtual ICollection<VehicleRequest> VehicleRequests { get; set; }
    }
}