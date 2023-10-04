using CarBookingBE.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(): base("name=CarBooking")
        {
            Configuration.ProxyCreationEnabled = true;
        }
        public virtual DbSet<Account> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<AccountRole> UserRoles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DepartmentMember> DepartmentsMembers { get; set;}
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<RequestAttachment> RequestAttachments { get; set; }
        public virtual DbSet<RequestComment> RequestComments { get; set; }
        public virtual DbSet<RequestWorkflow> RequestWorkflows { get; set; }
        public virtual DbSet<RequestShare> RequestShares { get; set; }
        public virtual DbSet<RequestCommentAttachment> RequestCommentAttachments { get; set; }
        public virtual DbSet<Rotation> Rotations { get; set; }
        public virtual DbSet<VehicleRequest> VehicleRequests { get; set; }
    }
}