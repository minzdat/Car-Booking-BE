using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CarBookingBE.Services
{
    public class RequestAttachmentService
    {
        private MyDbContext db = new MyDbContext();
        public Result<List<RequestAttachmentDTO>> GetAttachmentByRequestId (string id)
        {
            List <RequestAttachmentDTO> requestAttachmentList = db.RequestAttachments.Where(ra => ra.RequestId.ToString() == id && ra.IsDeleted == false)
                 .Select(ra => new RequestAttachmentDTO() {
                     Id = ra.Id,
                     Path = ra.Path
                 })
                 .ToList();
            return new Result<List<RequestAttachmentDTO>>(true, "Get Request Attachments Success", requestAttachmentList);
        }


        public Result<RequestAttachment> CreateAttachment(HttpPostedFile file, Guid requestId)
        {
            Request request = db.Requests.SingleOrDefault(r => r.Id == requestId && r.IsDeleted == false);
            if (request == null)
            {
                return new Result<RequestAttachment>(false, "Request Not Exist");
            }
            /*if (files.Length == 0)
            {
                return new Result<List<RequestAttachment>>(false, "Attachment is required");
            }
            foreach (HttpPostedFileBase file in files)*/
            if (file != null)
            {
                List<RequestAttachment> requestAttachments = new List<RequestAttachment>();
                RequestAttachment requestAttachment = new RequestAttachment();
                requestAttachment.IsDeleted = false;
                requestAttachment.RequestId = requestId;
                var rs = uploadFile(file, request.RequestCode, requestId);
                if (!rs.Success)
                {
                    return new Result<RequestAttachment>(false, rs.Message);
                }
                requestAttachment.Path = rs.Data;
                db.RequestAttachments.Add(requestAttachment);
                requestAttachments.Add(requestAttachment);
                db.SaveChanges();
            }

            return new Result<RequestAttachment>(true, "Create Attachments Success");
        }

        public Result<RequestAttachment> EditAttachment(HttpPostedFile file, Guid requestId)
        {
            DeleteAllAttachments(requestId);

            var requestAttachment = CreateAttachment(file, requestId);
            /*Request request = db.Requests.SingleOrDefault(r => r.Id == requestId && r.IsDeleted == false);

            if (request == null)
            {
                return new Result<RequestAttachment>(false, "Request Not Exist");
            }

            RequestAttachment requestAttachment = new RequestAttachment();
            if (file != null)
            {
                var rs = uploadFile(file, request.RequestCode, requestId);
                if (!rs.Success)
                {
                    return new Result<RequestAttachment>(false, rs.Message);
                }
                
                requestAttachment.IsDeleted = false;
                requestAttachment.RequestId = requestId;
                requestAttachment.Path = rs.Data;
                db.RequestAttachments.Add(requestAttachment);
                db.SaveChanges();
            }*/
            return new Result<RequestAttachment>(true, "Edit Attachments Success", requestAttachment.Data);
        }

        public Result<string> uploadFile(HttpPostedFile postedFile, string requestCode, Guid requestId)
        {
            string newPath = "Files/Attachments/" + requestCode + "/" + postedFile.FileName;
            List<RequestAttachment> requestAttachments = db.RequestAttachments.Where(ra => ra.IsDeleted == false).ToList();
            foreach (RequestAttachment req in requestAttachments)
            {
                if (req.Path == newPath && req.RequestId == requestId)
                {
                    return new Result<string>(false, "File: " + postedFile.FileName + " is exist in this Request");
                }
            }
            string[] acceptExtensionImg = { ".png", ".jpg", ".jpeg", ".pdf", ".csv", ".doc", ".docx", ".pptx", ".ppt", ".txt", "xls", "xlsx" };
            if (postedFile == null || postedFile.FileName.Length == 0)
            {
                return new Result<string>(false, "Missing file !");
            }
            if (!acceptExtensionImg.Contains(Path.GetExtension(postedFile.FileName)))
            {
                return new Result<string>(false, "Not support file type !");
            }
            if (postedFile.ContentLength > (20 * 1024 * 1024))
            {
                return new Result<string>(false, "The maximum size of file is 20MB !");
            }
            string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Attachments"), requestCode);
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
            postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
            return new Result<string>(true, "Upload file successfully !", $"Files/Attachments/{requestCode}/{postedFile.FileName}");
        }

        public Result<RequestAttachment> DeleteAttachment(Guid Id)
        {
            RequestAttachment requestAttachment = db.RequestAttachments.SingleOrDefault(ra => ra.Id == Id);
            if (requestAttachment == null || requestAttachment.IsDeleted == true)
            {
                return new Result<RequestAttachment>(false, "Request Attachment Not Found");
            }
            requestAttachment.IsDeleted = true;
            db.SaveChanges();
            return new Result<RequestAttachment>(true, "Delete Success Request Attachment has Id = " + requestAttachment.Id);
        }

        public Result<string> DeleteAllAttachments (Guid requestId)
        {
            var existRequestAttachments = db.RequestAttachments.Where(e => e.IsDeleted == false && e.RequestId == requestId).ToList();
            foreach (RequestAttachment existRequestAttachment in existRequestAttachments)
            {
                DeleteAttachment(existRequestAttachment.Id);
            }
            return new Result<string>(true, "Delete All Attachments Success!");
        }

    }
}