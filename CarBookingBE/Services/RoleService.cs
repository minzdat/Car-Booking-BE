using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data;
using CarBookingBE.DTOs;

namespace CarBookingBE.Services
{
    public class RoleService
    {
        MyDbContext _db = new MyDbContext();
        public Result<Pagination<Role>> getAllRole(int page, int limit)
        {
            try
            {
                var initRList = _db.Roles.Where(r => r.IsDeleted == false)
                .OrderBy(r => r.Id);
                var rList = initRList
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
                if (rList.Any())
                {
                    var rolePagination = new Pagination<Role> {
                        PerPage = limit,
                        CurrentPage = page,
                        TotalPage = (initRList.ToList().Count + limit - 1) / limit,
                        ListData = rList
                    };
                    return new Result<Pagination<Role>>(true, "Get all roles successfully !", rolePagination);
                }
                return new Result<Pagination<Role>>(false, "There's no data !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Pagination<Role>>(false, "Internal error !");
            }
        }

        public Result<Role> getRoleById(int rId)
        {
            try
            {
                var gRole = _db.Roles.FirstOrDefault(r => r.IsDeleted == false && r.Id == rId);
                if (gRole != null)
                {
                    return new Result<Role>(true, "Get role successfully !", gRole);
                }
                return new Result<Role>(false, "Data does not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Role>(false, "Internal error !");
            }
        }

        public Result<Role> addRole(Role role)
        {
            try
            {
                if (role == null || role.Title == null)
                {
                    return new Result<Role>(false, "Missing parameter !");
                }
                /*var reusable = _db.Roles.Where(r => r.IsDeleted == true && r.Title == role.Title).FirstOrDefault();
                if (reusable != null)
                {
                    reusable.IsDeleted = false;
                    _db.SaveChanges();
                    return new Result<Role>(true, "Add role for user successfully !", reusable);
                }*/

                //check duplicate
                var checkExist = _db.Roles.FirstOrDefault(r => r.IsDeleted == false && r.Title == role.Title);
                if (checkExist == null)
                {
                    _db.Roles.Add(role);
                    _db.SaveChanges();
                    return new Result<Role>(true, "Add role successfully !", role);
                }
                return new Result<Role>(false, "This title of role has already existed !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Role>(false, "Internal error !");
            }
        }

        public Result<Role> editRole(int rId, Role role)
        {
            try
            {
                if (role == null || role.Title == null)
                {
                    return new Result<Role>(false, "Missing parameter !");
                }

                //check duplicate role title
                var isExisted = _db.Roles.FirstOrDefault(r => r.IsDeleted == false && r.Title == role.Title);
                if (isExisted != null)
                {
                    return new Result<Role>(false, "This title's already existed !");
                }
                var eRole = _db.Roles.FirstOrDefault(r => r.IsDeleted == false && r.Id == rId);
                if (eRole != null)
                {
                    eRole.Title = role.Title;
                    _db.SaveChanges();
                    return new Result<Role>(true, "Edit role title successfully !", eRole);
                }
                return new Result<Role>(false, "Data does not exist");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Role>(false, "Internal error !");
            }
        }

        public Result<Role> deleteRole(string strRoleId)
        {
            try
            {
                var rId = int.Parse(strRoleId);
                var dRole = _db.Roles.FirstOrDefault(r => r.IsDeleted == false && r.Id == rId);
                if (dRole != null)
                {
                    var deleteUserRole = _db.UserRoles.Where(ur => ur.IsDeleted == false && ur.RoleId == rId).ToList();
                    if(deleteUserRole.Any())
                    {
                        return new Result<Role>(false, "Cannot delete, this role has some related data in other places !");
                    }
                    /*foreach (var item in deleteUserRole)
                    {
                        item.IsDeleted = true;
                    }
                    dRole.IsDeleted = true;*/
                    _db.Roles.Remove(dRole);
                    _db.SaveChanges();
                    return new Result<Role>(true, "Delete role successfully !");
                }
                return new Result<Role>(false, "Data does not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Role>(false, "Internal error !");
            }
        }
    }
}