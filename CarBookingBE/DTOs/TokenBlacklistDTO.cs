using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class TokenBlacklistDTO
    {
        public string UserId { get; set; }
        public string JwtToken { get; set; }
        public TokenBlacklistDTO() { }
        public TokenBlacklistDTO(string uid, string jwt)
        {
            UserId = uid;
            JwtToken = jwt;
        }
        public override string ToString()
        {
            return $"{UserId},{JwtToken}";
        }
    }
}