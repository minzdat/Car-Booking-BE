using CarBookingTest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingBE.Models
{
    [Table("VehicleRequest")]
    public class VehicleRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }
        public Guid? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public Account User { get; set; }
        public string DriverMobile { get; set; }
        public string DriverCarplate { get; set; }
        public int? RotaionId { get; set; }
        [ForeignKey("RotaionId")]
        public Rotation Rotation { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public bool Type { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}