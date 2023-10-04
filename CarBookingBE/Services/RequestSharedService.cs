using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.Services
{
    public class RequestSharedService
    {
        private MyDbContext db = new MyDbContext();
        public Result<RequestShare> createRequestShare(Guid? requestId, Guid? userId)
        {
            var request = db.Requests.SingleOrDefault(r => r.Id == requestId && r.IsDeleted == false);
            if (request == null)
            {
                return new Result<RequestShare>(false, "Request Not Found");
            }
            var user = db.Users.SingleOrDefault(u => u.Id == userId && u.IsDeleted == false);
            if (user == null)
            {
                return new Result<RequestShare>(false, "User Not Found");
            }
            if (request.SenderId == userId)
            {
                return new Result<RequestShare>(false, "Can't share for Sender User");
            }
            if (request.ReceiverId == userId)
            {
                return new Result<RequestShare>(false, "Can't share for Receiver User");
            }
            var requestWorkflows = db.RequestWorkflows.Where(rwf => rwf.RequestId == requestId && rwf.IsDeleted == false).Select(rwf => rwf.UserId).ToList();
            if (requestWorkflows.Contains(userId))
            {
                return new Result<RequestShare>(false, "Can't share for Approver");
            }

            var requestShare = db.RequestShares.SingleOrDefault(rs => rs.IsDeleted == false && rs.RequestId == requestId && rs.UserId == userId);
            if (requestShare != null)
            {
                return new Result<RequestShare>(false, "Request is shared with this User");
            }
            RequestShare newRequestShare = new RequestShare();
            newRequestShare.RequestId = requestId;
            newRequestShare.UserId = userId;
            db.RequestShares.Add(newRequestShare);
            db.SaveChanges();
            return new Result<RequestShare>(true, "Share Request Success", newRequestShare);
        }
    }
}