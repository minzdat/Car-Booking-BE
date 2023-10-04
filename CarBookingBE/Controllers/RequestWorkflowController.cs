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
    [RoutePrefix("api/request/workflow")]
    public class RequestWorkflowController : ApiController
    {
        RequestWorkflowService requestWorkflowService = new RequestWorkflowService();

        [Route("requestId={requestId}")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetRequestWorkflowByRequestId (string requestId)
        {
            var requestWorkflows = requestWorkflowService.GetRequestWorkflowByRequestId(requestId);
            if (!requestWorkflows.Success)
            {
                return BadRequest(requestWorkflows.Message);
            }
            return Ok(requestWorkflows);
        }

        /*[Route("create")]
        [HttpPost]
        public IHttpActionResult CreateRequestWorkflow(List<RequestWorkflow> requestWorkflows)
        {
            var requestWorkflow = requestWorkflowService.CreateRequestWorkflow(requestWorkflows);
            if (!requestWorkflow.Success)
            {
                return BadRequest(requestWorkflow.Message);
            }
            return Ok(requestWorkflow);
        }*/
    }
}