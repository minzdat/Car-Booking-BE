using CarBookingBE.Services;
using CarBookingTest.Models;
using System.Web.Http;
using System.Windows.Documents;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/userRole")]
    public class UserRoleController : ApiController
    {
        UserRoleService userRoleService = new UserRoleService();

        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAllUserRole(int page, int limit)
        {
            return Ok(userRoleService.getAllUserRole(page, limit));
        }

        [HttpGet]
        [Route("find")]
        public IHttpActionResult getUserRoleById(AccountRole accRole)
        {
            return Ok(userRoleService.getUserRoleById(accRole));
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult addUserRole([FromBody] AccountRole accRole)
        {
            return Ok(userRoleService.addUserRole(accRole));
        }

        [HttpPut]
        [Route("edit")]
        public IHttpActionResult editUserRole([FromBody] AccountRole accRole)
        {
            return Ok(userRoleService.editUserRole(accRole));
        }

        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult deleteUserRole([FromBody] AccountRole accRole)
        {
            return Ok(userRoleService.deleteUserRole(accRole));
        }

        [HttpGet]
        [Route("roles-uid/{userId}")]
        public IHttpActionResult getRolesDetailByUserId(string userId)
        {
            return Ok(userRoleService.getRolesDetailByUserId(userId));
        }

        [HttpGet]
        [Route("roles-list/{userId}")]
        public IHttpActionResult getRolesByUserId(string userId)
        {
            return Ok(userRoleService.getRolesByUserId(userId));
        }

        [HttpPost]
        [Route("add-roles/{userId}")]
        public IHttpActionResult addUserRoles(string userId, string[] departments)
        {
            return Ok(userRoleService.addUserRoles(userId, departments));
        }

        [HttpGet]
        [Route("all-approvers/{did}")]
        public IHttpActionResult getApprovers(string did)
        {
            return Ok(userRoleService.getApprovers(did));
        }
    }
}