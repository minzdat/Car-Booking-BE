using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace CarBookingBE.DTOs
{
    public class RoleConstants
    {
        public string ADMIN = "ADMIN";
        public string ADMINISTRATIVE = "ADMINISTRATIVE";
        public string APPROVER = "APPROVER";
        public string EMPLOYEE = "EMPLOYEE";
        public string SECURITY = "SECURITY";
        public List<string> Roles = new List<string>();
        public RoleConstants() { }
        public RoleConstants(bool admin, bool administrative, bool approver, bool employee, bool security)
        {
            if(admin) Roles.Add(ADMIN);
            if (administrative) Roles.Add(ADMINISTRATIVE);
            if (approver) Roles.Add(APPROVER);
            if (employee) Roles.Add(EMPLOYEE);
            if (security) Roles.Add(SECURITY);
        }
    }
}