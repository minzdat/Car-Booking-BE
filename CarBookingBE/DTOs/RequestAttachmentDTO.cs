using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class RequestAttachmentDTO
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
    }
}