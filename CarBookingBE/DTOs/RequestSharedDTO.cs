using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class RequestSharedDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public AccountDTO User { get; set; }
        public RequestDTO Request { get; set; }
    }
}