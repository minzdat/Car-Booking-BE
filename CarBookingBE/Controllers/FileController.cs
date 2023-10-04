using CarBookingBE.DTOs;
using CarBookingBE.Services;
using CarBookingBE.Utils;
using CarBookingTest.Utils;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {
        FileService fileService = new FileService();
        RoleConstants roleConstants = new RoleConstants();
        UtilMethods util = new UtilMethods();

        [HttpGet]
        [Route("xlxs-requests")]
        [JwtAuthorize]
        public HttpResponseMessage writeExcel()
        {
            var check = util.getCurId();
            if(!check.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = false, Message = "Login required !" });
            }
            var curId = check.Data;
            return Request.CreateResponse(HttpStatusCode.OK, fileService.writeToExcelAndDownload(curId));
        }

        [HttpGet]
        [Route("create-qrcode")]
        public HttpResponseMessage createQRCode()
        {
            string link = "http://localhost:3000";
            return Request.CreateResponse(HttpStatusCode.OK, fileService.createQRCode(link));
        }

        [HttpGet]
        [Route("pdf-request/{requestId}")]
        public HttpResponseMessage createPdfRequest(string requestId)
        {
            return fileService.writeDownPdf(requestId);
        }

        [HttpGet]
        [Route("delete-files-temp")]
        public IHttpActionResult deleteFile()
        {
            var filePath = "Files/Avatar/temp";
            return Ok(fileService.deleteAllFilesInFolder(filePath));
        }

        /*[HttpGet]
        [Route("download")]
        public HttpResponseMessage downloadFileExcel()
        {
            *//*var id = util.getCurId();
            if(!id.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = false, Message = "Login required !" });
            }
            var curId = id.Data;*//*
            return Request.CreateResponse(HttpStatusCode.OK, fileService.downloadFileExcel("3f8ef640-4895-4f06-99e8-a829d25a35c1", "test"));
        }

        [HttpGet]
        [Route("downloadpdf")]
        public HttpResponseMessage downloadFilePdf()
        {
            return Request.CreateResponse(HttpStatusCode.OK, fileService.downloadFilePdf("test"));
        }*/

        [HttpPost]
        [Route("upload")]
        [JwtAuthorize]
        public HttpResponseMessage uploadAvatar()
        {
            /*var isAuthorized = util.isAuthorized(new RoleConstants(true, true, true, true, true));
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            var curId = isAuthorized.Data;*/
            var data = util.getCurId().Data;
            if(data == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            var curId = data;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadAvatar(curId.ToString(), httpRequest.Files[0]));
            }
            return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadAvatar(curId.ToString(), null));
        }
        
        [HttpPost]
        [Route("upload-finish")]
        [JwtAuthorize]
        public HttpResponseMessage finishUpload()
        {
            HttpRequest request = HttpContext.Current.Request;
            var userId = request.Form["userId"];
            var fileName = request.Form["fileName"];
            return Request.CreateResponse(HttpStatusCode.OK, fileService.copyAvatarFromTemp(userId, fileName));
        }

        [HttpPost]
        [Route("upload-temp")]
        [JwtAuthorize]
        public HttpResponseMessage uploadAvatarTemp()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadAvatarTemp(httpRequest.Files[0]));
            }
            return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadAvatarTemp(null));
        }

        [HttpPost]
        [Route("signature-finish")]
        [JwtAuthorize]
        public HttpResponseMessage finishUploadSignature()
        {
            HttpRequest request = HttpContext.Current.Request;
            var userId = request.Form["userId"];
            var fileName = request.Form["fileName"];
            return Request.CreateResponse(HttpStatusCode.OK, fileService.copySignatureFromTemp(userId, fileName));
        }

        [HttpPost]
        [Route("signature-temp")]
        [JwtAuthorize]
        public HttpResponseMessage uploadSignatureTemp()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadSignatureTemp(httpRequest.Files[0]));
            }
            return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadSignatureTemp(null));
        }
    }
}