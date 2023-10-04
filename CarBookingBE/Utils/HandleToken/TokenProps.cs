using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CarBookingTest.Utils
{
    public class TokenProps
    {
        public string secretKey = "carbookinggnikoobraccarbookingteamtwoowtmaetfaskjdfsdfkjasdfkjsdkjfdkjajue";
        public string issuer = "carbookingissuer";
        public string audience = "carbookingaudience";
        public int expirationMinutes = 60 * 24;
        public string tokenForLogout;
        public string GenerateJwtToken(string secretKey, string issuer, string audience, int expirationMinutes, Claim[] claims)
        {
            // Create token credentials
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = signingCredentials
            };

            // Create token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Generate token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Serialize token to string
            return tokenHandler.WriteToken(token);
        }
        public bool VerifyJwtToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // Convert the secret key to bytes
                ValidateIssuer = false, // Set to true if you want to validate the issuer
                ValidateAudience = false, // Set to true if you want to validate the audience
                ClockSkew = TimeSpan.Zero // Set the tolerance for the token's expiration time
            };

            try
            {
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Token signature is valid
                return true;
            }
            catch
            {
                // Token signature validation failed
                return false;
            }
        }
        public string UpdateTokenExpiration(string existingToken, string secretKey, int expirationMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            // Read the existing token
            var existingSecurityToken = tokenHandler.ReadToken(existingToken) as JwtSecurityToken;

            // Create a new claims identity with the existing claims
            var newClaimsIdentity = new ClaimsIdentity(existingSecurityToken.Claims);

            // Create a new token descriptor with the updated claims identity and expiration time
            var newTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = newClaimsIdentity,
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Generate the new token
            var newSecurityToken = tokenHandler.CreateToken(newTokenDescriptor);

            // Write the new token as a string
            var newToken = tokenHandler.WriteToken(newSecurityToken);

            return newToken;
        }
        public DateTime? GetTokenExpiration(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Read the token and retrieve the expiration time claim
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var expirationClaim = securityToken.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expirationClaim != null && long.TryParse(expirationClaim.Value, out long expirationTime))
            {
                // Convert the expiration time from Unix timestamp to DateTime
                var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds(expirationTime).UtcDateTime;
                return expirationDateTime;
            }

            return null; // Token does not contain expiration time claim or it's not in the expected format
        }
    }
}