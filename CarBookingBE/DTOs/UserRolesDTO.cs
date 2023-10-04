using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class UserRolesDTO
    {
        public RoleDTO Role { get; set; }
    }
}