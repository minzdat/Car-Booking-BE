using CarBookingBE.Services;
using CarBookingTest.Models;
using System.Web.Http;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/role")]
    public class RoleController : ApiController
    {
        RoleService roleService = new RoleService();

        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAllRole(int page, int limit)
        {
            return Ok(roleService.getAllRole(page, limit));
        }

        [HttpGet]
        [Route("find/{id}")]
        public IHttpActionResult getRoleById(int id)
        {
            return Ok(roleService.getRoleById(id));
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult addDepartment([FromBody] Role role)
        {
            return Ok(roleService.addRole(role));
        }

        [HttpPut]
        [Route("edit/{id}")]
        public IHttpActionResult editRole(int id, [FromBody] Role role)
        {
            return Ok(roleService.editRole(id, role));
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult deleteRole(string id)
        {
            return Ok(roleService.deleteRole(id));
        }
    }
}