using CarBookingBE.DTOs;
using CarBookingBE.Services;
using CarBookingBE.Utils;
using CarBookingBE.Utils.HandleToken;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;
using System.Windows;

namespace CarBookingTest.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        UserService userService = new UserService();
        FileService fileService = new FileService();
        UtilMethods util = new UtilMethods();

        [HttpPost]
        [Route("login")]
        public IHttpActionResult loginUser([FromBody] LoginDTO user)
        {
            return Ok(userService.loginService(user));
        }

        [HttpPost]
        [Route("register")]
        [JwtAuthorize]
        public HttpResponseMessage registerUser()
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            var curId = isAuthorized.Data;
            var httpRequest = HttpContext.Current.Request;
            Account user = new Account();
            user.Email = httpRequest.Unvalidated.Form["Email"];
            user.Password = httpRequest.Unvalidated.Form["Password"];
            user.FirstName = httpRequest.Unvalidated.Form["FirstName"];
            user.LastName = httpRequest.Unvalidated.Form["LastName"];
            user.Sex = bool.Parse(httpRequest.Unvalidated.Form["Sex"]);
            user.EmployeeNumber = httpRequest.Unvalidated.Form["EmployeeNumber"];
            user.JobTitle = httpRequest.Unvalidated.Form["JobTitle"];
            string roleId = httpRequest.Unvalidated.Form["Role"];
            if (httpRequest.Files.Count == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, userService.registerService(curId, httpRequest.Files[0], user, roleId));
            }
            return Request.CreateResponse(HttpStatusCode.OK, userService.registerService(curId, null, user, roleId));
        }

        [HttpGet]
        [Route("all")]
        [JwtAuthorize]
        public IHttpActionResult getAllUser(int page, int limit)
        {
            return Ok(userService.getUsersService(page, limit));
        }

        [HttpGet]
        [Route("profile/{id}")]
        [JwtAuthorize]
        public HttpResponseMessage getProfile(string id)
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            var curId = isAuthorized.Data != null ? isAuthorized.Data : new Guid();
            // users can view profile of themselves
            if (curId == Guid.Parse(id) || isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, userService.getProfileService(id));
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
        }


        [HttpPut]
        [Route("edit-post-file/{idEdit}")]
        [JwtAuthorize]
        public HttpResponseMessage editProfileWithPostFile(string idEdit)
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            var curId = isAuthorized.Data != null ? isAuthorized.Data : new Guid();

            // users can edit profile of themselves or admin
            if (curId == Guid.Parse(idEdit) || isAuthorized.Success)
            {
                var httpRequest = HttpContext.Current.Request;
                var formData = httpRequest.Unvalidated.Form;
                if (httpRequest.Files.Count == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, userService.editProfileWithPostFile(curId, httpRequest.Files[0], idEdit, formData));
                }
                return Request.CreateResponse(HttpStatusCode.OK, userService.editProfileWithPostFile(curId, null, idEdit, formData));
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
        }

        [HttpGet]
        [Route("logout")]
        [JwtAuthorize]
        public IHttpActionResult logoutUser()
        {
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var uid = util.getCurId();
                if(!uid.Success)
                {
                    return Ok(new { Success = false, Message = "Do not have current user, log out failed !" });
                }
                var userId = uid.Data.ToString();
                TokenBlacklist.BlacklistToken(userId, token);
                return Ok(new { Success = true, Message = "Log out successfully !" });
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return Ok(new { Success = true, Message = "Internal error, log out failed !" });
            }
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage addUser([FromBody] AccountForAddDTO user)
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            //var curId = isAuthorized.Data;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, userService.addUserService(user));
            }
            return Request.CreateResponse(HttpStatusCode.OK, userService.addUserService(user));
        }

        [HttpPut]
        [Route("edit/{idEdit}")]
        [JwtAuthorize]
        public HttpResponseMessage editProfile(string idEdit, [FromBody] AccountForAddDTO user)
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            var curId = isAuthorized.Data != null ? isAuthorized.Data : new Guid();

            // users can edit profile of themselves or admin
            if (curId == Guid.Parse(idEdit) || isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, userService.editProfileService(idEdit, user));
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult deleteUser(string id)
        {
            return Ok(userService.deleteUser(id));
        }

        [HttpGet]
        [Route("check-jwt")]
        [JwtAuthorize]
        public IHttpActionResult checkToken()
        {
            return Ok("Token works !");
        }

        [HttpPost]
        [Route("signature")]
        [JwtAuthorize]
        public IHttpActionResult setSignature(Account user)
        {
            return Ok(userService.setSignature(user));
        }
    }
}