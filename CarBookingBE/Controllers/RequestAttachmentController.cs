using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CarBookingBE.Services;
using CarBookingTest.Models;
using CarBookingTest.Utils;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request/attachment")]
    public class RequestAttachmentController : ApiController
    {
        RequestAttachmentService requestAttachmentService = new RequestAttachmentService(); 

        [Route("get-all")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetAllRequestAttachment()
        {
            return Ok();
        }

        [Route("requestId={requestId}")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetAttachmentsByRequestId(string requestId)
        {
            return Ok(requestAttachmentService.GetAttachmentByRequestId(requestId));
        }

        /*[Route("create/requestId={requestId}")]
        [HttpPost]
        public IHttpActionResult CreateAttachment(string requestId)
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {

                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    var rAS = requestAttachmentService.CreateAttachment(httpRequest.Files[i], requestId);
                    if (!rAS.Success)
                    {
                        return BadRequest(rAS.Message);
                    }

                }
            }
            return Ok();
            

        }*/
    }
}