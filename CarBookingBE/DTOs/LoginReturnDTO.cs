using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class LoginReturnDTO
    {
        public AccountLoginReturnDTO userInfo { get; set; }
        public string jwtToken { get; set; }
        public List<UserRolesDTO> userRole { get; set; }
        public LoginReturnDTO(AccountLoginReturnDTO userInfo, string jwtToken, List<UserRolesDTO> userRole)
        {
            this.userInfo = userInfo;
            this.jwtToken = jwtToken;
            this.userRole = userRole;
        }
    }
}