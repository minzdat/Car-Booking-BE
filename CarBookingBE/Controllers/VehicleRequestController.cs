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
using CarBookingBE.Models;
using CarBookingBE.Services;
using CarBookingTest.Models;
using CarBookingTest.Utils;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request/vehicle")]
    public class VehicleRequestController : ApiController
    {

        VehicleRequestService vehicleRequestService = new VehicleRequestService();

        [Route("requestId={requestId}")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult getVehicleRequest(string requestId)
        {
            var vehicleRequest = vehicleRequestService.getVehicleRequest(requestId);
            if (!vehicleRequest.Success)
            {
                return BadRequest(vehicleRequest.Message);
            }
            return Ok(vehicleRequest);
        }


        [Route("create")]
        [HttpPost]
        [JwtAuthorize]
        public IHttpActionResult createVehicleRequest(VehicleRequest vehicleRequest)
        {
            var newVehicleRequest = vehicleRequestService.createVehicleRequest(vehicleRequest);
            if (!newVehicleRequest.Success)
            {
                return BadRequest(newVehicleRequest.Message);
            }
            return Ok(newVehicleRequest);
        }

        [Route("rotation")]
        [HttpGet]
        [JwtAuthorize]
        public IHttpActionResult getAllRotations()
        {
            var rotations = vehicleRequestService.getAllRotation();
            if (!rotations.Success)
            {
                return BadRequest(rotations.Message);
            }
            return Ok(rotations);
        }
    }
}