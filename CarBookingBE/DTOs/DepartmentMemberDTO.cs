using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class DepartmentMemberDTO
    {
        public Guid Id { get; set; }
        public string Position { get; set; }
        public AccountDTO User { get; set; }
        public DepartmentDTO Department { get; set; }
    }
}