using CarBookingBE.DTOs;
using CarBookingBE.Services;
using CarBookingTest.Models;
using System.Web.Http;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/departmentMember")]
    public class DepartmentMemberController : ApiController
    {
        DepartmentMemberService dms = new DepartmentMemberService();
        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAll(int page, int limit)
        {
            return Ok(dms.getAll(page, limit));
        }

        [HttpGet]
        [Route("position")]
        public IHttpActionResult getByPosition(string departmentId)
        {
            return Ok(dms.getByDepartmentId(departmentId));
        }

        [HttpGet]
        [Route("position")]
        public IHttpActionResult getByPosition(string departmentId, string position)
        {
            return Ok(dms.getByPositionWithDeparmentId(departmentId, position));
        }

        [HttpGet]
        [Route("find/{id}")]
        public IHttpActionResult getDepartment(string id)
        {
            return Ok(dms.getDepartmentMember(id));
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult addDepartment([FromBody] DepartmentMember dm)
        {
            return Ok(dms.addDepartmentMember(dm));
        }

        [HttpPut]
        [Route("edit/{id}")]
        public IHttpActionResult editDepartment(string id, [FromBody] DepartmentMember dm)
        {
            return Ok(dms.editDepartmentMember(id, dm));
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult deleteDepartment(string id)
        {
            return Ok(dms.deleteDepartmentMember(id));
        }
        
        [HttpGet]
        [Route("departments-uid/{userId}")]
        public IHttpActionResult getDepartmentsByUserId(string userId)
        {
            return Ok(dms.getDepartmentsByUserId(userId));
        }

        [HttpGet]
        [Route("manager/{departmentId}")]
        public IHttpActionResult getManagerByDepartment(string departmentId)
        {
            return Ok(dms.getManagerByDepartment(departmentId));
        }

        [HttpGet]
        [Route("supervisors/{departmentId}")]
        public IHttpActionResult getAllSupervisors(string departmentId)
        {
            return Ok(dms.getAllSupervisors(departmentId));
        }
    }
}