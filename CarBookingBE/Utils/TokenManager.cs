using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.Utils
{
    public class TokenManager
    {
        private static readonly List<string> InvalidTokens = new List<string>();

        public bool IsTokenValid(string token)
        {
            // Check if the token is in the list of invalid tokens
            return !InvalidTokens.Contains(token);
        }

        public void InvalidateToken(string token)
        {
            // Add the token to the list of invalid tokens
            InvalidTokens.Add(token);
        }
    }
}