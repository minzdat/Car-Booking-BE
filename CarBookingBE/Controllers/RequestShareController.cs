using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CarBookingBE.Services;
using CarBookingTest.Models;
using CarBookingTest.Utils;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request/share")]
    public class RequestShareController : ApiController
    {
        RequestSharedService requestSharedService = new RequestSharedService();

        [Route("create")]
        [HttpPost]
        [JwtAuthorize]
        public IHttpActionResult createRequestShare(RequestShare requestShare)
        {
            var newRequestShare = requestSharedService.createRequestShare(requestShare.RequestId, requestShare.UserId);
            if (!newRequestShare.Success)
            {
                return BadRequest(newRequestShare.Message);
            }
            return Ok(newRequestShare.Message);
        }
    }
}