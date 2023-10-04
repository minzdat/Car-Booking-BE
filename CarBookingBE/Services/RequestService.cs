using CarBookingBE.DTOs;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data;
using System.Data.Entity;
using CarBookingBE.Utils;
using System.Globalization;
using CarBookingTest.Utils;

namespace CarBookingBE.Services
{
    public class RequestService
    {
        private MyDbContext db = new MyDbContext();

        UtilMethods utilMethods = new UtilMethods();


        public Result<IQueryable<RequestDTO>> GetAllRequests()
        {
            var requireRoles = new RoleConstants(true, true, false, false, false);
            var isAuthorized = utilMethods.isAuthorized(requireRoles);
            var userLoginId = isAuthorized.Data;
            /*if (!isAuthorized.Success)
            {
                return new Result<IQueryable<RequestDTO>>(false, isAuthorized.Message);
            }*/
            /*            if (!userLoginId.Equals(Guid.Parse(userId)))
                        {
                            return new Result<IQueryable<RequestDTO>>(false, "User Not Match");
                        }*/

            if (isAuthorized.Success)
            {
                var queries = db.Requests
                    .Include(r => r.SenderUser)
                    .Include(receiver => receiver.ReceiveUser)
                    .Where(request => request.IsDeleted == false)
                    .Select(req => new RequestDTO()
                    {
                        Id = req.Id,
                        RequestCode = req.RequestCode,
                        SenderUser = new AccountDTO()
                        {
                            Id = req.SenderUser.Id,
                            FirstName = req.SenderUser.FirstName,
                            LastName = req.SenderUser.LastName
                        },
                        Created = req.Created,
                        Department = new DepartmentDTO()
                        {
                            Name = req.Department.Name
                        },
                        ReceiveUser = new AccountDTO()
                        {
                            Id = req.ReceiveUser.Id,
                            FirstName = req.ReceiveUser.FirstName,
                            LastName = req.ReceiveUser.LastName
                        },
                        UsageFrom = req.UsageFrom,
                        UsageTo = req.UsageTo,
                        Status = req.Status,
                        RequestWorkflows = db.RequestWorkflows.Where(rwf => rwf.RequestId == req.Id && rwf.IsDeleted == false).Select(rwf => rwf.UserId).ToList()
                    });
                return new Result<IQueryable<RequestDTO>>(true, "Get Data Success", queries);
            }
            else
            {
                /*var queries = db.Requests.Include(sender => sender.SenderUser).Include(receiver => receiver.ReceiveUser)*/
                var queries = db.Requests
                    .Include(sender => sender.SenderUser)
                    .Include(receiver => receiver.ReceiveUser)
                    .Include(rs => rs.RequestShares)
                    .Include(rwf => rwf.RequestWorkflows)
                    .Where(request => request.IsDeleted == false)
                    .Where(request => request.ReceiverId == userLoginId || request.RequestWorkflows.Any(rwf => rwf.UserId == userLoginId) || request.RequestShares.Any(rs => rs.UserId == userLoginId))
                    .Select(req => new RequestDTO()
                    {
                        Id = req.Id,
                        RequestCode = req.RequestCode,
                        SenderUser = new AccountDTO()
                        {
                            Id = req.SenderUser.Id,
                            FirstName = req.SenderUser.FirstName,
                            LastName = req.SenderUser.LastName
                        },
                        Created = req.Created,
                        Department = new DepartmentDTO()
                        {
                            Name = req.Department.Name
                        },
                        ReceiveUser = new AccountDTO()
                        {
                            Id = req.ReceiveUser.Id,
                            FirstName = req.ReceiveUser.FirstName,
                            LastName = req.ReceiveUser.LastName
                        },
                        UsageFrom = req.UsageFrom,
                        UsageTo = req.UsageTo,
                        Status = req.Status,
                        RequestWorkflows = db.RequestWorkflows.Where(rwf => rwf.RequestId == req.Id && rwf.IsDeleted == false).Select(rwf => rwf.UserId).ToList()
                    });
                return new Result<IQueryable<RequestDTO>>(true, "Get Data Success", queries);
            }
        }

        public Result<IQueryable<RequestDTO>> GetSentToMe()
        {
            var userLogin = utilMethods.getCurId();
            if (!userLogin.Success)
            {
                return new Result<IQueryable<RequestDTO>>(false, userLogin.Message);
            }

            var userLoginId = userLogin.Data;
            /* if (userLoginId.Data != Guid.Parse(userId))
             {
                 return new Result<IQueryable<RequestDTO>>(false, "ERROR: User not match");
             }*/
            var queries = db.Requests
                .Include(s => s.SenderUser)
                .Include(r => r.ReceiveUser)
                .Include(rwf => rwf.RequestWorkflows)
                .Where(request => request.IsDeleted == false)
                .Where(request => request.ReceiverId == userLoginId || request.RequestWorkflows.Any(rwf => rwf.UserId == userLoginId))
                .Select(req => new RequestDTO()
                {
                    Id = req.Id,
                    RequestCode = req.RequestCode,
                    SenderUser = new AccountDTO()
                    {
                        Id = req.SenderUser.Id,
                        FirstName = req.SenderUser.FirstName,
                        LastName = req.SenderUser.LastName
                    },
                    Created = req.Created,
                    Department = new DepartmentDTO()
                    {
                        Name = req.Department.Name
                    },
                    ReceiveUser = new AccountDTO()
                    {
                        Id = req.ReceiveUser.Id,
                        FirstName = req.ReceiveUser.FirstName,
                        LastName = req.ReceiveUser.LastName
                    },
                    UsageFrom = req.UsageFrom,
                    UsageTo = req.UsageTo,
                    Status = req.Status,
                    RequestWorkflows = db.RequestWorkflows.Where(rwf => rwf.RequestId == req.Id && rwf.IsDeleted == false).Select(rwf => rwf.UserId).ToList()
                });


            /*            if (queries.Count() == 0)
                        {
                            return new Result<List<RequestDTO>>(false, "Get Requests Failed");
                        }*/
            return new Result<IQueryable<RequestDTO>>(true, "Get Requests Success", queries);
        }

        public Result<IQueryable<RequestDTO>> GetSentToOthers()
        {
            var userLogin = utilMethods.getCurId();
            if (!userLogin.Success)
            {
                return new Result<IQueryable<RequestDTO>>(false, userLogin.Message);
            }

            var userLoginId = userLogin.Data;
            /*if (!userLoginId.Data.Equals(Guid.Parse(userLoginId)))
            {
                return new Result<IQueryable<RequestDTO>>(false, "ERROR: User not match");
            }*/
            var queries = db.Requests
                .Include(s => s.SenderUser)
                .Include(r => r.ReceiveUser)
                .Where(request => request.IsDeleted == false)
                .Where(request => request.SenderId == userLoginId)
                .Select(req => new RequestDTO()
                    {
                        Id = req.Id,
                        RequestCode = req.RequestCode,
                        SenderUser = new AccountDTO()
                        {
                            Id = req.SenderUser.Id,
                            FirstName = req.SenderUser.FirstName,
                            LastName = req.SenderUser.LastName
                        },
                        Created = req.Created,
                        Department = new DepartmentDTO()
                        {
                            Name = req.Department.Name
                        },
                        ReceiveUser = new AccountDTO()
                        {
                            Id = req.ReceiveUser.Id,
                            FirstName = req.ReceiveUser.FirstName,
                            LastName = req.ReceiveUser.LastName
                        },
                        UsageFrom = req.UsageFrom,
                        UsageTo = req.UsageTo,
                        Status = req.Status,
                        RequestWorkflows = db.RequestWorkflows.Where(rwf => rwf.RequestId == req.Id && rwf.IsDeleted == false).Select(rwf => rwf.UserId).ToList()
                });
            return new Result<IQueryable<RequestDTO>>(true, "Get Requests Success", queries);
        }

        public Result<IQueryable<RequestDTO>> GetSharedWithMe()
        {
            var userLogin = utilMethods.getCurId();
            if (!userLogin.Success)
            {
                return new Result<IQueryable<RequestDTO>>(false, userLogin.Message);
            }

            var userLoginId = userLogin.Data;
            /*if (!userLoginId.Data.Equals(Guid.Parse(userId)))
            {
                return new Result<IQueryable<RequestDTO>>(false, "ERROR: User not match");
            }*/
            var queries = db.Requests
                .Include(s => s.SenderUser)
                .Include(r => r.ReceiveUser)
                .Include(rs => rs.RequestShares)
                .Where(request => request.IsDeleted == false)
                .Where(request => request.RequestShares.Any(rs => rs.UserId == userLoginId))
                .Select(req => new RequestDTO()
                {
                    Id = req.Id,
                    RequestCode = req.RequestCode,
                    SenderUser = new AccountDTO()
                    {
                        Id = req.SenderUser.Id,
                        FirstName = req.SenderUser.FirstName,
                        LastName = req.SenderUser.LastName
                    },
                    Created = req.Created,
                    Department = new DepartmentDTO()
                    {
                        Name = req.Department.Name
                    },
                    ReceiveUser = new AccountDTO()
                    {
                        Id = req.ReceiveUser.Id,
                        FirstName = req.ReceiveUser.FirstName,
                        LastName = req.ReceiveUser.LastName
                    },
                    UsageFrom = req.UsageFrom,
                    UsageTo = req.UsageTo,
                    Status = req.Status,
                    RequestWorkflows = db.RequestWorkflows.Where(rwf => rwf.RequestId == req.Id && rwf.IsDeleted == false).Select(rwf => rwf.UserId).ToList()
                });
            return new Result<IQueryable<RequestDTO>>(true, "Get Requests Success", queries);
        }

        public Result<RequestDetailDTO> GetRequestById(string id)
        {
            /*List<RequestWorkflowDTO> requestWorkflow = db.RequestWorkflows.Where(r => r.RequestId.ToString() == id).Include(u => u.User)
                    .Select(rwf => new RequestWorkflowDTO()
                    {
                        Level = rwf.Level,
                        User = new AccountDTO()
                        {
                            Id = rwf.User.Id,
                            FirstName = rwf.User.FirstName,
                            LastName = rwf.User.LastName
                        }
                    }).ToList();*/
            RequestDetailDTO request = db.Requests
                .Include(s => s.SenderUser)
                .Include(r => r.ReceiveUser)
                .Where(r => r.Id.ToString() == id && r.IsDeleted == false)
                .Select(req => new RequestDetailDTO()
                {
                    Id = req.Id,
                    RequestCode = req.RequestCode,
                    SenderUser = new AccountDTO()
                    {
                        Id = req.SenderUser.Id,
                        FirstName = req.SenderUser.FirstName,
                        LastName = req.SenderUser.LastName,
                        Username = req.SenderUser.Username,
                        Email = req.SenderUser.Email,
                        AvatarPath = req.SenderUser.AvatarPath,
                        JobTitle = req.SenderUser.JobTitle
                    },
                    Created = req.Created,
                    Department = new DepartmentDTO()
                    {
                        Id = req.Department.Id,
                        Name = req.Department.Name
                    },
                    ReceiverUser = new AccountDTO()
                    {
                        Id = req.ReceiveUser.Id,
                        FirstName = req.ReceiveUser.FirstName,
                        LastName = req.ReceiveUser.LastName,
                        Username = req.ReceiveUser.Username,
                        Email = req.ReceiveUser.Email,
                        AvatarPath = req.ReceiveUser.AvatarPath,
                        JobTitle = req.ReceiveUser.JobTitle
                    },/*
                    RequestWorkflow = requestWorkflow,*/
                    UsageFrom = req.UsageFrom,
                    UsageTo = req.UsageTo,
                    Status = req.Status,
                    Mobile = req.Mobile,
                    CostCenter = req.CostCenter,
                    TotalPassengers = req.TotalPassengers,
                    PickTime = req.PickTime,
                    PickLocation = req.PickLocation,
                    Destination = req.Destination,
                    Reason = req.Reason,
                    Note = req.Note,
                    ApplyNote = req.ApplyNote

                })
                .SingleOrDefault();
            var allRequest = GetAllRequests().Data.ToList();
            if (!allRequest.Exists(r => r.Id == request.Id))
            {
                return new Result<RequestDetailDTO>(false, "Request Not Found");
            }
            if (request == null)
            {
                return new Result<RequestDetailDTO>(false, "Failed");
            }

            return new Result<RequestDetailDTO>(true, "Success", request);
        }

        public Result<Request> EditRequest(string id, Request requestEdit)
        {
            var requestId = Guid.Parse(id);
            var req = db.Requests.SingleOrDefault(r => r.Id == requestId && r.IsDeleted == false);
            if (req.Status != "Draft")  
            {
                if (req.Status != "Rejected")
                {
                    return new Result<Request>(false, "Request can't Edit");
                }
            }

            var requireRoles = new RoleConstants(true, true, false, false, false);
            var isAuthorized = utilMethods.isAuthorized(requireRoles);
            var userLoginId = isAuthorized.Data;
            if (!isAuthorized.Success && req.SenderId != userLoginId)
            {
                return new Result<Request>(false, "Permission Failed");
            }

            /*if (requestEdit.Status != "Draft" || requestEdit.Status == null)
            {
                requestEdit.Status = "Waiting for approval";
            }
            else
            {
                requestEdit.Status = "Draft";
            }*/
            if (req == null || req.IsDeleted == true)
            {
                return new Result<Request>(false, "Request Not Found");
            }

            if (Guid.Parse(id) != req.Id)
            {
                return new Result<Request>(false, "Request id not match");
            }

            if (requestEdit.DepartmentId != null) req.DepartmentId = requestEdit.DepartmentId;
            if (requestEdit.ReceiverId != null) req.ReceiverId = requestEdit.ReceiverId;
            if (requestEdit.Mobile != null) req.Mobile = requestEdit.Mobile;
            if (requestEdit.CostCenter != null) req.CostCenter = requestEdit.CostCenter;
            if (requestEdit.TotalPassengers != 0) req.TotalPassengers = requestEdit.TotalPassengers;
            if (requestEdit.UsageFrom != null) req.UsageFrom = requestEdit.UsageFrom;
            if (requestEdit.UsageTo != null) req.UsageTo = requestEdit.UsageTo;
            if (requestEdit.PickTime != null) req.PickTime = requestEdit.PickTime;
            if (requestEdit.PickLocation != null) req.PickLocation = requestEdit.PickLocation;
            if (requestEdit.Destination != null) req.Destination = requestEdit.Destination;
            if (requestEdit.Reason != null) req.Reason = requestEdit.Reason;
            if (requestEdit.ApplyNote != null) req.ApplyNote = requestEdit.ApplyNote;
            req.Status = "Waiting for approval";
            req.Created = DateTime.Now;
            db.SaveChanges();



            return new Result<Request>(true, "Edit Request Succcess!", req);
        }

        public Result<NewRequestDTO> CreateRequest(Request request)
        {
            request.Created = DateTime.Now;
            request.IsDeleted = false;
            request.RequestCode = GenerateRequestCode();
            if (request.Status != "Draft" || request.Status == null)
            {
                request.Status = "Waiting for approval";
            }
            else
            {
                request.Status = "Draft";
            }
            if (request.Mobile == null || request.CostCenter == null || request.TotalPassengers == null || request.UsageFrom == null
                || request.UsageTo == null || request.PickTime == null || request.PickLocation == null
                || request.Destination == null || request.Reason == null || request.ApplyNote == null)
            {
                return new Result<NewRequestDTO>(false, "Missing Fields");
            }

            if (db.Users.SingleOrDefault(u => u.Id == request.SenderId && u.IsDeleted == false) == null)
            {
                return new Result<NewRequestDTO>(false, "Sender User Not Exist");
            }

            if (db.Departments.SingleOrDefault(d => d.Id == request.DepartmentId && d.IsDeleted == false) == null)
            {
                return new Result<NewRequestDTO>(false, "Department Not Exist");
            }

            if (db.Users.SingleOrDefault(u => u.Id == request.ReceiverId && u.IsDeleted == false) == null)
            {
                return new Result<NewRequestDTO>(false, "Receiver User Not Exist");
            }

            if (request.UsageFrom > request.UsageTo)
            {
                return new Result<NewRequestDTO>(false, "Usage From must earlier than Usage To");
            }

            if (request.PickTime < request.UsageFrom || request.PickTime > request.UsageTo)
            {
                return new Result<NewRequestDTO>(false, "Pick Time must between Usage From and Usage To");
            }

            if (request.TotalPassengers <= 0)
            {
                return new Result<NewRequestDTO>(false, "Total Passengers must > 0");
            }



            db.Requests.Add(request);
            db.SaveChanges();
            NewRequestDTO requestDTO = new NewRequestDTO()
            {
                Id = request.Id,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                DepartmentId = request.DepartmentId,
                RequestCode = request.RequestCode,
                Created = request.Created,
                ApplyNote = request.ApplyNote,
                Status = request.Status,
                RequestAttachments = request.RequestAttachments,
                RequestWorkflows = request.RequestWorkflows
            };

            return new Result<NewRequestDTO>(true, "Create Request Success!", requestDTO);
        }

        public Result<Request> DeleteRequest(Guid id)
        {
            Request request = db.Requests.SingleOrDefault(r => r.Id == id);
            if (request == null || request.IsDeleted == true)
            {
                return new Result<Request>(false, "Request Not Found");
            }

            var requireRoles = new RoleConstants(true, true, false, false, false);
            var isAuthorized = utilMethods.isAuthorized(requireRoles);
            var userLoginId = isAuthorized.Data;
            if (isAuthorized.Success || request.SenderId == userLoginId)
            {
                request.IsDeleted = true;
                db.SaveChanges();
            } else
            {
                return new Result<Request>(false, "Permission Failed");
            }

            return new Result<Request>(true, "Delete Success Request has RequestCode = " + request.RequestCode);
        }

        public Result<Pagination<RequestDTO>> FilterRequest(IQueryable<RequestDTO> requestQueries, string search, string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {

            if (requestCode != null)
            {
                requestQueries = requestQueries.Where(req => req.RequestCode.Contains(requestCode));
            }

            if (createdFrom != null && createdTo != null)
            {
                DateTime _createdFrom = DateTime.Parse(createdFrom);
                DateTime _createdTo = DateTime.Parse(createdTo);
                DateTime _createdToPlus = _createdTo.AddDays(1);
                if (_createdFrom == _createdTo)
                {
                    requestQueries = requestQueries.Where(req => req.Created >= _createdFrom && req.Created < _createdToPlus);
                }
                else
                {
                    requestQueries = requestQueries.Where(req => req.Created >= _createdFrom && req.Created <= _createdTo);
                }
            }
            else if ((createdFrom != null && createdTo == null) || (createdFrom == null && createdTo != null))
            {
                return new Result<Pagination<RequestDTO>>(false, "createdFrom and createdTo are required!");
            }

            if (senderId != null)
            {
                requestQueries = requestQueries.Where(req => req.SenderUser.Id.ToString() == senderId);
            }

            if (status != null)
            {
                requestQueries = requestQueries.Where(req => req.Status == status);
            }

            if (search != null)
            {
                requestQueries = requestQueries.Where(req => req.RequestCode.Contains(search) || req.Department.Name.Contains(search));
            }


            var totalPage = (requestQueries.ToList().Count + limit - 1) / limit;

            var requestList = requestQueries.OrderByDescending(req => req.Created).Skip(getSkip(page, limit)).Take(limit).ToList();
            var Pagination = new Pagination<RequestDTO>()
            {
                ListData = requestList,
                PerPage = limit,
                CurrentPage = page,
                TotalPage = totalPage
            };

            Pagination<RequestDTO> result = Pagination;
            return new Result<Pagination<RequestDTO>>(true, "Filter Success", result);
        }

        public Result<Request> ActionRequest(string Id, string Note, string action)
        {
            var listOfActions = new List<string>() { "Approved", "Rejected", "Canceled" };

            if (!listOfActions.Contains(action))
            {
                return new Result<Request>(false, "Invalid Action Request! ");
            }

            var request = db.Requests.SingleOrDefault(r => r.Id.ToString() == Id && r.IsDeleted == false);
            if (request == null)
            {
                return new Result<Request>(false, "Request Not Found");
            }

            if (request.Status != "Approved" && action == "Canceled")
            {
                return new Result<Request>(false, "Request Status: Approved is required for this action");
            }

            var requireRolesCancel = new RoleConstants(true, true, false, false, false);
            var isAuthorizedCancel = utilMethods.isAuthorized(requireRolesCancel);

            var userLoginId = isAuthorizedCancel.Data;

            if (request.Status == "Approved" && action == "Canceled")
            {

                if (isAuthorizedCancel.Success)
                {
                    request.Status = action;
                    db.SaveChanges();
                    return new Result<Request>(true, request.Status + " Request Success!");
                }
                else
                {
                    return new Result<Request>(false, "Permission Failed!");
                }
            }

            if (request.Status != "Waiting for approval")
            {
                return new Result<Request>(false, "Request Status: " + request.Status + " is required");
            }

            //**------------Role Admin----------------**//
            var requireRolesAdmin = new RoleConstants(true, false, false, false, false);
            var isAuthorizedAdmin = utilMethods.isAuthorized(requireRolesAdmin);
            if (isAuthorizedAdmin.Success)
            {
                if (Note != null)
                {
                    request.Note = Note;
                }
                request.Status = action;
                db.SaveChanges();
                return new Result<Request>(true, request.Status + " Request Success");
            }

            //**------------Role Admin----------------**//

            var requireRolesApprover = new RoleConstants(true, true, true, false, false);
            var isAuthorizedApprover = utilMethods.isAuthorized(requireRolesApprover);
            if (isAuthorizedApprover.Success)
            {
                RequestWorkflow requestWorkflow = db.RequestWorkflows.SingleOrDefault(rw => rw.IsDeleted == false && rw.UserId == userLoginId && rw.RequestId.ToString() == Id);
                if (requestWorkflow == null)
                {
                    return new Result<Request>(false, "This User doesn't is approver for this Request");
                }


                /*if (request.Status == "Rejected" && action == "Draft")
                {
                    return new Result<Request>(false, "Request is " + request.Status);
                }*/
                if (Note != null)
                {
                    request.Note = Note;
                }
                request.Status = action;
                db.SaveChanges();
            }
            else
            {
                return new Result<Request>(false, "Permission Failed!");
            }




            return new Result<Request>(true, request.Status + " Request Success");
        }

        // ---------------------------Function----------------------------------------
        protected string GenerateRequestCode()
        {
            string year = DateTime.Now.Year.ToString();
            string companyCode = "OPS";
            string module = "CAR";
            string month = DateTime.Now.Month.ToString("d2");
            string day = DateTime.Now.Day.ToString("d2");
            string requestCodeBase = year + companyCode + "-" + module + "-" + month + day + "-";
            List<string> requestCodeList = new List<string>();
            List<int> requestCodeNumbers = new List<int>();
            int maxNumber = 0;

            foreach (Request request in db.Requests)
            {
                if (request.RequestCode.Contains(requestCodeBase))
                {
                    requestCodeList.Add(request.RequestCode);
                }
            }

            if (requestCodeList.Count() != 0)
            {
                foreach (string requestCode in requestCodeList)
                {
                    string requestCodeString = requestCode.Substring(requestCode.Length - 3);
                    requestCodeNumbers.Add(int.Parse(requestCodeString));
                }
                maxNumber = requestCodeNumbers.Max();
            }

            return requestCodeBase + (maxNumber + 1).ToString("000");
        }

        public Result<string> CheckKeysRequired(string[] allKeys)
        {
            if (!allKeys.Contains("SenderId")) return new Result<string>(false, "SenderId is required!");
            if (!allKeys.Contains("ReceiverId")) return new Result<string>(false, "ReceiverId is required!");
            if (!allKeys.Contains("DepartmentId")) return new Result<string>(false, "DepartmentId is required!");
            if (!allKeys.Contains("Mobile")) return new Result<string>(false, "Mobile is required!");
            if (!allKeys.Contains("CostCenter")) return new Result<string>(false, "CostCenter is required!");
            if (!allKeys.Contains("TotalPassengers")) return new Result<string>(false, "TotalPassengers is required!");
            if (!allKeys.Contains("UsageFrom")) return new Result<string>(false, "UsageFrom is required!");
            if (!allKeys.Contains("UsageTo")) return new Result<string>(false, "UsageTo is required!");
            if (!allKeys.Contains("PickTime")) return new Result<string>(false, "PickTime is required!");
            if (!allKeys.Contains("PickLocation")) return new Result<string>(false, "PickLocation is required!");
            if (!allKeys.Contains("Destination")) return new Result<string>(false, "Destination is required!");
            if (!allKeys.Contains("Reason")) return new Result<string>(false, "Reason is required!");
            if (!allKeys.Contains("ApplyNote")) return new Result<string>(false, "ApplyNote is required!");/*
            if (!allKeys.Contains("ListOfUserId")) return new Result<string>(false, "ListOfUserId is required");*/


            return new Result<string>(true, "Ok");
        }




        //--------------------Pagination ----------------------------------
        protected int getSkip(int pageIndex, int limit)
        {
            return (pageIndex - 1) * limit;
        }





    }




}