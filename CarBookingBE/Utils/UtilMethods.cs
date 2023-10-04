using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using CarBookingBE.DTOs;
using CarBookingTest.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CarBookingBE.Utils
{
    public class UtilMethods
    {
        MyDbContext _db = new MyDbContext();
        public bool stringValid(string value)
        {
            return value.Trim().Length > 0;
        }
        public Result<Guid> getCurId()
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                HttpContext httpContext = HttpContext.Current;
                var jwt = httpContext.Request.Headers["Authorization"];
                if (jwt == null)
                {
                    return new Result<Guid>(false, "Token not found in Headers !");
                }
                //Trace.WriteLine(jwt);
                var jwtTokenObj = tokenHandler.ReadJwtToken(jwt.Substring(7)); //ignore "Bearer "
                string curId = "";
                foreach (var claim in jwtTokenObj.Claims)
                {
                    //Trace.WriteLine($"{claim.Type}: {claim.Value}");
                    if (claim.Type.Equals("CurId"))
                    {
                        curId = claim.Value;
                        break;
                    }
                }
                return new Result<Guid>(true, "Get curId successfully !", Guid.Parse(curId));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Guid>(false, "Internal error !");
            }
        }
        public Result<Guid> isAuthorized(RoleConstants roles)
        {
            List<string> requiredRoles = roles.Roles;
            try
            {
                var curIdObj = getCurId();
                if (curIdObj.Success == false)
                {
                    return new Result<Guid>(false, "Get current Id failed, login required !");
                }
                var curId = curIdObj.Data;
                var userRoleList = userRoles(curId);
                if (userRoleList.Success == false)
                {
                    return new Result<Guid>(false, "Get list roles of current user failed !", curId);
                }
                if (userRoleList.Data.Any())
                {
                    foreach (var item in requiredRoles)
                    {
                        if (userRoleList.Data.Contains(item))
                        {
                            return new Result<Guid>(true, "Accepted, current user has required role !", curId);
                        }
                    }
                }
                return new Result<Guid>(false, "User does not have required role !", curId);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Guid>(false, "Internal error !");
            }
        }

        public Result<List<string>> userRoles(Guid curId)
        {
            try
            {
                var uRoles = new List<string>();
                var getUserRoles = _db.UserRoles
                    .Where(u => u.IsDeleted == false && u.UserId == curId)
                    .Select(u => new UserRolesDTO
                    {
                        Role = new RoleDTO
                        {
                            Id = u.Role.Id,
                            Title = u.Role.Title
                        }
                    })
                    .ToList();
                if (!getUserRoles.Any())
                {
                    return new Result<List<string>>(false, "User does not have any roles !");
                }
                foreach (var item in getUserRoles)
                {
                    uRoles.Add(item.Role.Title);
                }
                return new Result<List<string>>(true, "Get user roles successfully !", uRoles);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<string>>(false, "Internal error !");
            }
        }

        public bool userHasRole(Guid userId, string roleTitle)
        {
            var hasRole = _db.UserRoles.Where(u => u.IsDeleted == false && u.UserId == userId && u.Role.Title == roleTitle).FirstOrDefault();
            if (hasRole == null)
            {
                return false;
            }
            return true;
        }

        public bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }
    }
}