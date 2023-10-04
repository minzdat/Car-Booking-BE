using CarBookingBE.DTOs;
using CarBookingBE.Models;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CarBookingBE.Services
{
    public class VehicleRequestService
    {
        MyDbContext db = new MyDbContext();
        UtilMethods utilMethods = new UtilMethods();

        public Result<VehicleRequestDTO> getVehicleRequest(string requestId)
        {
            var RequestId = Guid.Parse(requestId);
            var vehicleRequest = db.VehicleRequests
                .Include(v => v.User)
                .Include(v => v.Rotation)
                .Select(v => new VehicleRequestDTO()
                {
                    User = new AccountDTO()
                    {
                        FirstName = v.User.FirstName,
                        LastName = v.User.LastName
                    },
                    DriverCarplate = v.DriverCarplate,
                    DriverMobile = v.DriverMobile,
                    Rotation = v.Rotation,
                    Reason = v.Reason,
                    Note = v.Note,
                    Type = v.Type,
                    IsDeleted = v.IsDeleted,
                    RequestId = v.RequestId
                })
                .FirstOrDefault(v => v.IsDeleted == false && v.RequestId == RequestId);


            if (vehicleRequest == null)
            {
                return new Result<VehicleRequestDTO>(false, "Vehicle Request Not Found");
            }
            return new Result<VehicleRequestDTO>(true, "Get Success", vehicleRequest);
        }
       
        public Result<VehicleRequest> createVehicleRequest(VehicleRequest vehicleRequest)
        {
            var isAuthorized = utilMethods.isAuthorized(new RoleConstants(true, true, false, false, false));
            /*var newVehicleRequest = new VehicleRequest();*/
            if (isAuthorized.Success)
            {
                if (vehicleRequest.RequestId == null)
                {
                    return new Result<VehicleRequest>(false, "Miss Parameters");
                }
                Request request = db.Requests.SingleOrDefault(r => r.IsDeleted == false
                    && r.Id == vehicleRequest.RequestId && r.Status == "Approved");
                if (request == null)
                {
                    return new Result<VehicleRequest>(false, "Request Not Found");
                }

                if (vehicleRequest.Type == true)
                {
                    /*var user = db.DepartmentsMembers.SingleOrDefault(dm => dm.IsDeleted == false && dm.UserId == vehicleRequest.DriverId && dm.DepartmentId == );*/
                    var user = db.Users.SingleOrDefault(u => u.IsDeleted == false && u.Id == vehicleRequest.DriverId);
                    if (user == null)
                    {
                        return new Result<VehicleRequest>(false, "User Not Found");
                    }

                    var rotaion = db.Rotations.SingleOrDefault(ro => ro.IsDeleted == false && ro.Id == vehicleRequest.RotaionId);
                    if (rotaion == null)
                    {
                        return new Result<VehicleRequest>(false, "RotationId Not Found");
                    }

                    request.Status = "Done";
                    
                } else
                {
                    request.Status = "Done";
                }
                
                /*newVehicleRequest.DriverId = vehicleRequest.DriverId;
                newVehicleRequest.RequestId = vehicleRequest.RequestId;
                newVehicleRequest.DriverMobile = vehicleRequest.DriverMobile;
                newVehicleRequest.DriverCarplate = vehicleRequest.DriverCarplate;
                newVehicleRequest.RotaionId = vehicleRequest.RotaionId;
                newVehicleRequest.Rotation = vehicleRequest.Rotation;
                newVehicleRequest.Reason = vehicleRequest.Reason;
                newVehicleRequest.Note = vehicleRequest.Note;
                newVehicleRequest.Type = vehicleRequest.Type;
                newVehicleRequest.IsDeleted = false;*/

                db.VehicleRequests.Add(vehicleRequest);
                db.SaveChanges();
            }
            else
            {
                return new Result<VehicleRequest>(false, "User Permission Failed!");
            }

            
            return new Result<VehicleRequest>(true, "Create Vehicle Request Success", vehicleRequest);
        }

        public Result<List<Rotation>> getAllRotation()
        {
            var rotations = db.Rotations.Where(r => r.IsDeleted == false).OrderBy(r => r.Type).ToList();
            return new Result<List<Rotation>>(true, "Get Success", rotations);
        }

        public Result<string> deleteVehicleRequest(Guid requestId)
        {
            var vehicleRequest = db.VehicleRequests.SingleOrDefault(v => v.IsDeleted == false && v.RequestId == requestId);
            if (vehicleRequest == null)
            {
                return new Result<string>(false, "Vehicle Not Found");
            }
            db.VehicleRequests.Remove(vehicleRequest);
            db.SaveChanges();
            return new Result<string>(true, "Delete Success");
        }

    }
}