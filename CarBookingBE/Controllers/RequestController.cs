using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using Newtonsoft.Json;
using CarBookingBE.Services;
using CarBookingTest.Utils;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request")]
    public class RequestController : ApiController
    {
        RequestService requestService = new RequestService();
        RequestAttachmentService requestAttachmentService = new RequestAttachmentService();
        RequestWorkflowService requestWorkflowService = new RequestWorkflowService();
        VehicleRequestService vehicleRequestService = new VehicleRequestService();
        RequestCommentService requestCommentService = new RequestCommentService();


        // GET: api/Request
        [Route("get-all")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetRequests(string search, string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = requestService.GetAllRequests();
            if (!requestList.Success)
            {
                return BadRequest(requestList.Message);
            }
            return Ok(requestService.FilterRequest(requestList.Data, search, requestCode, createdFrom, createdTo, senderId, status, page, limit));
        }

        // GET: api/Request/5
        [Route("Id={id}")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetRequest(string id)
        {
            var request = requestService.GetRequestById(id);
            if (!request.Success)
            {
                return BadRequest(request.Message);
            }
            return Ok(request);
        }

        // GET: Sent to me
        [Route("sent-to-me")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetSentToMe(string search, string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = requestService.GetSentToMe();
            if (!requestList.Success)
            {
                return BadRequest(requestList.Message);
            }
            return Ok(requestService.FilterRequest(requestList.Data, search, requestCode, createdFrom, createdTo, senderId, status, page, limit));
        }

        // GET: Sent to others
        [Route("sent-to-others")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetSentToOthers(string search, string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = requestService.GetSentToOthers();
            if (!requestList.Success)
            {
                return BadRequest(requestList.Message);
            }
            return Ok(requestService.FilterRequest(requestList.Data, search, requestCode, createdFrom, createdTo, senderId, status, page, limit));
        }

        //GET: Shared with me Not completed ------------------------
        [Route("shared-with-me")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult GetSharedWithMe(string search, string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = requestService.GetSharedWithMe();
            if (!requestList.Success)
            {
                return BadRequest(requestList.Message);
            }
            return Ok(requestService.FilterRequest(requestList.Data, search, requestCode, createdFrom, createdTo, senderId, status, page, limit));
        }

        // PUT: api/Request/5
        [Route("Id={id}")]
        [HttpPut]
        [JwtAuthorize]
        public IHttpActionResult EditRequest(string id)
        {
            var httpRequest = HttpContext.Current.Request;
            Request request = new Request();
            if (httpRequest.Unvalidated.Form["SenderId"] != null) request.SenderId = Guid.Parse(httpRequest.Unvalidated.Form["SenderId"]);
            if (httpRequest.Unvalidated.Form["DepartmentId"] != null) request.DepartmentId = Guid.Parse(httpRequest.Unvalidated.Form["DepartmentId"]);
            if (httpRequest.Unvalidated.Form["ReceiverId"] != null) request.ReceiverId = Guid.Parse(httpRequest.Unvalidated.Form["ReceiverId"]);
            if (httpRequest.Unvalidated.Form["Mobile"] != null) request.Mobile = httpRequest.Unvalidated.Form["Mobile"];
            if (httpRequest.Unvalidated.Form["CostCenter"] != null) request.CostCenter = httpRequest.Unvalidated.Form["CostCenter"];
            if (httpRequest.Unvalidated.Form["TotalPassengers"] != null) request.TotalPassengers = int.Parse(httpRequest.Unvalidated.Form["TotalPassengers"]);
            if (httpRequest.Unvalidated.Form["UsageFrom"] != null) request.UsageFrom = DateTime.Parse(httpRequest.Unvalidated.Form["UsageFrom"]);
            if (httpRequest.Unvalidated.Form["UsageTo"] != null) request.UsageTo = DateTime.Parse(httpRequest.Unvalidated.Form["UsageTo"]);
            if (httpRequest.Unvalidated.Form["PickTime"] != null) request.PickTime = DateTime.Parse(httpRequest.Unvalidated.Form["PickTime"]);
            if (httpRequest.Unvalidated.Form["PickLocation"] != null) request.PickLocation = httpRequest.Unvalidated.Form["PickLocation"];
            if (httpRequest.Unvalidated.Form["Destination"] != null) request.Destination = httpRequest.Unvalidated.Form["Destination"];
            if (httpRequest.Unvalidated.Form["Reason"] != null) request.Reason = httpRequest.Unvalidated.Form["Reason"];
            if (httpRequest.Unvalidated.Form["ApplyNote"] != null) request.ApplyNote = bool.Parse(httpRequest.Unvalidated.Form["ApplyNote"]);
            if (httpRequest.Unvalidated.Form["Note"] != null) request.Note = httpRequest.Unvalidated.Form["Note"];
            var requestEdit = requestService.EditRequest(id, request);
            if (!requestEdit.Success)
            {
                return BadRequest(requestEdit.Message);
            }
            var requestId = requestEdit.Data.Id;
            if (httpRequest.Unvalidated.Form["ListOfUserId[]"] != null)
            {
                // Edit RequestWorkflow
                var listOfUserId = httpRequest.Params.GetValues("ListOfUserId[]");

                var createRequestWorkflow = requestWorkflowService.EditRequestWorkflow(requestId, listOfUserId);
                if (!createRequestWorkflow.Success)
                {
                    return BadRequest(createRequestWorkflow.Message);
                }
            }
            

            // Edit RequestAttachment
            if (httpRequest.Files.Count > 0)
            {
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    
                    var createAttachment = requestAttachmentService.EditAttachment(httpRequest.Files[i], requestId);
                    if (!createAttachment.Success)
                    {
                        return BadRequest(createAttachment.Message);
                    }
                }
                /*var requestAttachments = requestAttachmentService.GetAttachmentByRequestId(requestId.ToString());
                foreach(RequestAttachmentDTO requestAttachment in requestAttachments.Data)
                {
                    if (!newRequestAttachments.Exists(nr => nr.Path == requestAttachment.Path))
                    {
                        requestAttachmentService.deleteAttachment(requestAttachment.Id);
                    }
                }*/
            }

            return Ok(requestEdit);
        }

        // DELETE: api/Request/5
        [Route("{id}")]
        [HttpDelete]
        [JwtAuthorize]
        public IHttpActionResult DeleteRequest(string id)
        {
            Guid requestId = Guid.Parse(id);
            requestWorkflowService.DeleteAllRequestWorkflows(requestId);
            requestAttachmentService.DeleteAllAttachments(requestId);
            vehicleRequestService.deleteVehicleRequest(requestId);
/*            requestCommentService.DeleteAllRequestComments(requestId);*/
            requestService.DeleteRequest(requestId);
            return Ok();
        }

        [Route("create")]
        [HttpPost]
        [JwtAuthorize]
        public IHttpActionResult CreateRequest()
        {
            var httpRequest = HttpContext.Current.Request;
            var checkKeys = requestService.CheckKeysRequired(httpRequest.Form.AllKeys);
            if (!checkKeys.Success)
            {
                return BadRequest(checkKeys.Message);
            }
            Request request = new Request();
            request.SenderId = Guid.Parse(httpRequest.Unvalidated.Form["SenderId"]);
            request.DepartmentId = Guid.Parse(httpRequest.Unvalidated.Form["DepartmentId"]);
            request.ReceiverId = Guid.Parse(httpRequest.Unvalidated.Form["ReceiverId"]);
            request.Mobile = httpRequest.Unvalidated.Form["Mobile"];
            request.CostCenter = httpRequest.Unvalidated.Form["CostCenter"];
            request.TotalPassengers = int.Parse(httpRequest.Unvalidated.Form["TotalPassengers"]);
            request.UsageFrom = DateTime.Parse(httpRequest.Unvalidated.Form["UsageFrom"]);
            request.UsageTo = DateTime.Parse(httpRequest.Unvalidated.Form["UsageTo"]);
            request.PickTime = DateTime.Parse(httpRequest.Unvalidated.Form["PickTime"]);
            request.PickLocation = httpRequest.Unvalidated.Form["PickLocation"];
            request.Destination = httpRequest.Unvalidated.Form["Destination"];
            request.Reason = httpRequest.Unvalidated.Form["Reason"];
            request.ApplyNote = bool.Parse(httpRequest.Unvalidated.Form["ApplyNote"]);
            if (httpRequest.Unvalidated.Form["Status"] != null) request.Status = httpRequest.Unvalidated.Form["Status"];
            
            var newRequest = requestService.CreateRequest(request);

            if (newRequest.Success)
            {
                // Create RequestWorkflow
                var requestId = newRequest.Data.Id;
                var listOfUserId = httpRequest.Params.GetValues("ListOfUserId[]");
                if (listOfUserId == null)
                {
                    listOfUserId = httpRequest.Params.GetValues("ListOfUserId");
                }
                /*List<RequestWorkflow> requestWorkflows = new List<RequestWorkflow>();
                foreach (var userId in httpRequest.Params.GetValues("ListOfUserId"))
                {
                    RequestWorkflow requestWorkflow = new RequestWorkflow();
                    requestWorkflow.UserId = Guid.Parse(userId.ToString());
                    requestWorkflow.RequestId = newRequest.Data.Id;
                    requestWorkflows.Add(requestWorkflow);
                }
*/
                var createRequestWorkflow = requestWorkflowService.CreateRequestWorkflow(requestId, listOfUserId);
                if (!createRequestWorkflow.Success)
                {
                    return BadRequest(createRequestWorkflow.Message);
                }

                // Create RequestAttachment
                if (httpRequest.Files.Count > 0)
                {
                    for (int i = 0; i < httpRequest.Files.Count; i++)
                    {
                        var createAttachment = requestAttachmentService.CreateAttachment(httpRequest.Files[i], newRequest.Data.Id);
                        if (!createAttachment.Success)
                        {
                            return BadRequest(createAttachment.Message);
                        }

                    }
                }
            }
            return Ok(newRequest);
        }

        [Route("action/Id={Id}")]
        [HttpPut]
        [JwtAuthorize]
        public IHttpActionResult ActionRequest(string Id)
        {
            var httpRequest = HttpContext.Current.Request;
            string action = httpRequest.Unvalidated.Form["action"];
            string Note = httpRequest.Unvalidated.Form["Note"];
            var requestWorkflow = requestWorkflowService.ActionRequest(Guid.Parse(Id), action);
            if (!requestWorkflow.Success)
            {
                return BadRequest(requestWorkflow.Message);
            }

            var checkWorkflow = requestWorkflowService.CheckWorkflow(requestWorkflow.Data);
            if (checkWorkflow.Success)
            {
                var actionRequest = requestService.ActionRequest(Id, Note, action);
                if (!actionRequest.Success)
                {
                    return BadRequest(actionRequest.Message);
                }
                return Ok(actionRequest.Message);

            }


            return Ok(requestWorkflow.Message);
        }

        [Route("action/cancel/Id={Id}")]
        [HttpPut]
        [JwtAuthorize]
        public IHttpActionResult ActionCancel(string Id)
        {
            var httpRequest = HttpContext.Current.Request;
            string action = httpRequest.Unvalidated.Form["action"];
            string Note = httpRequest.Unvalidated.Form["Note"];
            var actionRequest = requestService.ActionRequest(Id, Note, action);
            if (!actionRequest.Success)
            {
                return BadRequest(actionRequest.Message);
            }
            return Ok(actionRequest.Message);
            /*var requestWorkflow = requestWorkflowService.ActionRequest(Guid.Parse(Id), action);
            if (!requestWorkflow.Success)
            {
                return BadRequest(requestWorkflow.Message);
            


            var checkWorkflow = requestWorkflowService.CheckWorkflow(requestWorkflow.Data);
            if (checkWorkflow.Success)
            {
                

            }


            return Ok(requestWorkflow.Message);*/
        }

    }
}