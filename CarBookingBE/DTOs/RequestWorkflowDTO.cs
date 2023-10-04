using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class RequestWorkflowDTO
    {
        public Guid Id { get; set; }
        public AccountDTO User { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        public string Position { get; set; }
    }
}