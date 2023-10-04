using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class AccountApproversDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public string JobTitle { get; set; }

        public string Position { get; set; }
    }
}