using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class PositionDepartmentDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string Code { get; set; }
        public Guid? UnderDepartment { get; set; }
        public string Description { get; set; }
        public string Manager { get; set; }
        public List<string> Supervisors { get; set; }
        public bool ManEm { get; set; }
        public bool SupEm { get; set; }
    }
}