using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Windows;
using CarBookingBE.Utils;
using CarBookingBE.Utils.HandleToken;
using Microsoft.IdentityModel.Tokens;

namespace CarBookingTest.Utils
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        TokenProps props = new TokenProps();
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var token = actionContext.Request.Headers.Authorization?.Parameter;

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var secretKey = props.secretKey; // Replace with the actual secret key used for token validation

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = false, // Set to true if you want to validate the issuer
                    ValidateAudience = false, // Set to true if you want to validate the audience
                    ClockSkew = TimeSpan.Zero // Set the tolerance for the token's expiration time
                };

                // Validate the token
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

                // Perform additional authorization checks based on the claims in the token if needed
                return !TokenBlacklist.IsTokenBlacklisted(token);
            }
            catch
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            // Custom unauthorized response message
            var unauthorizedMessage = "Token needed, login required !";

            // Set the response with the unauthorized message and status code
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, unauthorizedMessage);
        }
    }
}