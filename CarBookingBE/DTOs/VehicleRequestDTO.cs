using CarBookingBE.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class VehicleRequestDTO
    {
        public AccountDTO User { get; set; }
        public Guid? RequestId { get; set; }
        public bool IsDeleted { get; set; }
        public string DriverMobile { get; set; }
        public string DriverCarplate { get; set; }
        public Rotation Rotation { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public bool Type { get; set; }
    }
}