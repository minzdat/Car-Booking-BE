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
using CarBookingBE.Utils;
using CarBookingTest.Models;
using CarBookingTest.Utils;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request")]
    public class RequestCommentController : ApiController
    {
        RequestCommentService requestCommentService = new RequestCommentService();
        UtilMethods utilMethods = new UtilMethods();

        [Route("comment/requestId={requestId}")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetAllCommentByRequestId (string requestId)
        {
            var requestComments = requestCommentService.GetAllCommentByRequestId(requestId);
            if (!requestComments.Success)
            {
                return BadRequest(requestComments.Message);
            }
            return Ok(requestComments);
        }

        [Route("comment/requestId={requestId}")]
        [HttpPost]
        [JwtAuthorize]
        /*[System.Web.Mvc.ValidateInput(false)]*/
        public IHttpActionResult CreateComment (string requestId)
        {
            var userLoginId = utilMethods.getCurId();
            var httpRequest = HttpContext.Current.Request;
            RequestComment requestComment = new RequestComment();
            requestComment.UserId = userLoginId.Data;
            requestComment.Content = httpRequest.Unvalidated.Form["comment"];
            requestComment.Created = DateTime.Now;
            requestComment.RequestId = Guid.Parse(requestId);
            requestComment.IsDeleted = false;

            var comments = requestCommentService.CreateComment(requestComment, requestId);  
            if (!comments.Success)
            {
                return BadRequest(comments.Message);
            }

            if (httpRequest.Files.Count > 0)
            {
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    var commentAttachments = requestCommentService.CreateCommentAttachment(httpRequest.Files[i], requestComment, userLoginId.Data.ToString());
                    if (!commentAttachments.Success)
                    {
                        return BadRequest(commentAttachments.Message);
                    }
                }
            }


            return Ok(comments);
        }
    }
}