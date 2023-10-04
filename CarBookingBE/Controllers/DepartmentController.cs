using CarBookingBE.DTOs;
using CarBookingBE.Services;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/department")]
    public class DepartmentController : ApiController
    {
        DepartmentService departmentService = new DepartmentService();

        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult getAll()
        {
            return Ok(departmentService.getAll());
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAll(int page, int limit)
        {
            return Ok(departmentService.getAll(page, limit));
        }

        [HttpGet]
        [Route("find/{id}")]
        public IHttpActionResult getDepartment(string id)
        {
            return Ok(departmentService.getDepartment(id));
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult addDepartment([FromBody] Department department)
        {
            return Ok(departmentService.addDepartment(department));
        }

        [HttpPut]
        [Route("edit/{id}")]
        public IHttpActionResult editDepartment(string id, [FromBody] PositionDepartmentDTO department)
        {
            return Ok(departmentService.editDepartment(id, department));
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult deleteDepartment(string id)
        {
            return Ok(departmentService.deleteDepartment(id));
        }
    }
}