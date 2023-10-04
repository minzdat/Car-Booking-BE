using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.PeerToPeer;

namespace CarBookingBE.Services
{
    public class UserRoleService
    {
        MyDbContext _db = new MyDbContext();

        public Result<Pagination<AccountRole>> getAllUserRole(int page, int limit)
        {
            try
            {
                var urList = _db.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(r => r.IsDeleted == false)
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
                if (urList.Any())
                {
                    var userRolePagination = new Pagination<AccountRole>
                    {
                        PerPage = limit,
                        CurrentPage = page,
                        TotalPage = (urList.Count + limit - 1) / limit,
                        ListData = urList
                    };
                    return new Result<Pagination<AccountRole>>(true, "Get all roles successfully !", userRolePagination);
                }
                return new Result<Pagination<AccountRole>>(false, "There's no data !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Pagination<AccountRole>>(false, "Internal error !");
            }
        }

        public Result<AccountRole> getUserRoleById(AccountRole accRole)
        {
            try
            {
                if (accRole == null || accRole.Id == null)
                {
                    return new Result<AccountRole>(false, "Missing or Invalid id field !");
                }
                var arId = accRole.Id;
                var existedUR = _db.UserRoles
                    .Include(r => r.User)
                    .Include(r => r.Role)
                    .FirstOrDefault(ur => ur.IsDeleted == false && ur.Id == arId);
                if (existedUR != null)
                {
                    return new Result<AccountRole>(true, "Get data by id successfully !", existedUR);
                }
                return new Result<AccountRole>(false, "There's no data !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<AccountRole>(false, "Internal error !");
            }
        }
        public Result<AccountRole> addUserRole(AccountRole accRole)
        {
            try
            {
                if(accRole == null || accRole.UserId == null || accRole.RoleId == null)
                {
                    return new Result<AccountRole>(false, "Missing parameter(s) or Invalid id field !");
                }
                var uId = accRole.UserId;
                var rId = accRole.RoleId;
                var user = _db.Users.Find(uId);
                var role = _db.Roles.Find(rId);
                if (user != null && role != null)
                {
                    /*var reusable = _db.UserRoles.Where(r => r.IsDeleted == true && r.UserId == uId && r.RoleId == rId).FirstOrDefault();
                    if(reusable != null)
                    {
                        reusable.IsDeleted = false;
                        _db.SaveChanges();
                        return new Result<AccountRole>(true, "Add role for user successfully !", reusable);
                    }*/

                    //check duplicate
                    var isExist = _db.UserRoles.Where(r => r.IsDeleted == false && r.UserId == uId && r.RoleId == rId).FirstOrDefault();
                    if (isExist == null)
                    {
                        var newUserRole = new AccountRole { UserId = uId, RoleId = rId };
                        _db.UserRoles.Add(newUserRole);
                        _db.SaveChanges();
                        return new Result<AccountRole>(true, "Add role for user successfully !", newUserRole);
                    }
                    return new Result<AccountRole>(false, "Add fail, this user's already had the input role !");
                }
                return new Result<AccountRole>(false, "User or role do not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<AccountRole>(false, "Internal error !");
            }
        }

        public Result<string[]> addUserRoles(string strUserId, string[] addRolesList)
        {
            var userId = Guid.Parse(strUserId);
            var checkExistUser = _db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Id == userId);
            if (checkExistUser == null)
            {
                return new Result<string[]>(false, "User does not exist !");
            }
            if (addRolesList == null || addRolesList.Length == 0)
            {
                return new Result<string[]>(false, "Empty input roles list !");
            }
            var rs = getRolesByUserId(strUserId);
            if(!rs.Success)
            {
                return new Result<string[]>(false, rs.Message);
            }
            foreach (var strRoleId in rs.Data)
            {
                if(!addRolesList.Contains(strRoleId))
                {
                    deleteUserRoleById(strUserId, strRoleId);
                }
            }
            foreach (var strRoleId in addRolesList)
            {
                var roleId = int.Parse(strRoleId);
                var role = _db.UserRoles.FirstOrDefault(u => u.IsDeleted == false && u.RoleId == roleId && u.UserId == userId);
                if (role == null)
                {
                    var addSuccess = addUserRole(new AccountRole { UserId = userId, RoleId = roleId });
                    if (!addSuccess.Success) break;
                }
            }
            return new Result<string[]>(true, "Add roles for user successfully !", addRolesList);
        }

        public Result<string> deleteUserRoleById(string strUserId, string strRoleId)
        {
            var uId = Guid.Parse(strUserId);
            var rId = int.Parse(strRoleId);
            if(uId == null)
            {
                return new Result<string>(false, "Missing or invalid id of user !");
            }
            if(strRoleId == null)
            {
                return new Result<string>(false, "Missing or invalid id of role !");
            }
            var existUser = _db.Users.FirstOrDefault(u => u.Id == uId);
            if (existUser == null)
            {
                return new Result<string>(false, "User with input id does not exist");
            }
            var existRole = _db.Roles.FirstOrDefault(r => r.Id == rId);
            if(existRole == null)
            {
                return new Result<string>(false, "Role with input title does not exist !");
            }
            var delData = _db.UserRoles.FirstOrDefault(ur => ur.RoleId == rId && ur.UserId == uId && ur.IsDeleted == false);
            if(delData != null)
            {
                _db.UserRoles.Remove(delData);
                _db.SaveChanges();
                return new Result<string>(true, "Delete a user role successfully !");
            }
            return new Result<string>(false, "Not found data to delete !");
        }

        public Result<AccountRole> editUserRole(AccountRole accRole)
        {
            try
            {
                if (accRole == null || accRole.Id == null || accRole.UserId == null || accRole.RoleId == null)
                {
                    return new Result<AccountRole>(false, "Missing parameter(s) or Invalid id field !");
                }
                var urId = accRole.Id;
                var uId = accRole.UserId;
                var rId = accRole.RoleId;
                var userRole = _db.UserRoles.FirstOrDefault(ur => ur.IsDeleted == false && ur.Id == urId);
                if (userRole == null)
                {
                    return new Result<AccountRole>(false, "Data does not exist !");
                }
                var user = _db.Users.Find(uId);
                var role = _db.Roles.Find(rId);
                if (user != null && role != null)
                {
                    var existedUR = _db.UserRoles.FirstOrDefault(ur => ur.IsDeleted == false && ur.UserId == uId && ur.RoleId == rId);
                    if (existedUR != null)
                    {
                        return new Result<AccountRole>(false, "Edit fail, this user's already had the input role !");
                    }
                    userRole.UserId = uId;
                    userRole.RoleId = rId;
                    _db.SaveChanges();
                    return new Result<AccountRole>(true, "Edit role for user successfully !", existedUR);
                }
                return new Result<AccountRole>(false, "User or role do not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<AccountRole>(false, "Internal error !");
            }
        }

        public Result<AccountRole> deleteUserRole(AccountRole accRole)
        {
            try
            {
                if (accRole == null || accRole.Id == null)
                {
                    return new Result<AccountRole>(false, "Missing or Invalid id field !");
                }
                var arId = accRole.Id;
                var userRole = _db.UserRoles.FirstOrDefault(ur => ur.IsDeleted == false && ur.Id == arId);
                if (userRole == null)
                {
                    return new Result<AccountRole>(false, "Input data does not exist !");
                }
                userRole.IsDeleted = true;
                _db.SaveChanges();
                return new Result<AccountRole>(true, "Delete role for user successfully !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<AccountRole>(false, "Internal error !");
            }
        }

        public Result<List<string>> getRolesByUserId(string userId)
        {
            try
            {
                var uId = Guid.Parse(userId);
                var rolesById = new List<string>();
                var urs = _db.UserRoles.Where(ur => ur.UserId == uId).ToList();
                foreach (var item in urs)
                {
                    rolesById.Add(item.RoleId.ToString());
                }
                return new Result<List<string>>(true, "Get roles by userId successfully !", rolesById);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<string>>(false, "Get roles by userId failed !", new List<string>());
            }
        }

        public Result<List<UserRolesDTO>> getRolesDetailByUserId(string userId)
        {
            var uId = Guid.Parse(userId);
            var userRoles = _db.UserRoles
                .Where(ur => ur.IsDeleted == false && ur.UserId == uId)
                .Select(ur => new UserRolesDTO
                {
                    Role = new RoleDTO
                    {
                        Id = ur.Role.Id,
                        Title = ur.Role.Title
                    }
                })
                .ToList();
            if(!userRoles.Any())
            {
                return new Result<List<UserRolesDTO>>(false, "There's no data !");
            }
            return new Result<List<UserRolesDTO>>(true, "Get roles detail by user id successfully !", userRoles);
        }
        public Result<List<AccountApproversDTO>> getApprovers(string did)
        {
            var departmentId = Guid.Parse(did);
            var listApprover = _db.UserRoles
                .Where(u => u.User.IsDeleted == false)
                .Where(u => u.Role.Id == 1 || u.Role.Id == 2 || u.Role.Id == 3)
                .Select(ur => ur.User.Id)
                .Distinct()
                .ToList();
            if (!listApprover.Any())
            {
                return new Result<List<AccountApproversDTO>>(false, "There's no data to return !");
            }
            List<AccountApproversDTO> approvers = new List<AccountApproversDTO>();
            foreach (var approverId in listApprover)
            {
                var approver = _db.Users.Where(u => u.Id == approverId && u.DepartmentMembers.Any(us => us.DepartmentId == departmentId)).Select(u => new AccountApproversDTO
                {
                    Id = approverId,
                    Email = u.Email,
                    FullName = ((u.FirstName != null && u. LastName != null) || (u.FirstName.Trim().Length > 0 && u.LastName.Trim().Length > 0)) ? u.FirstName + " " + u.LastName : "",
                    JobTitle = u.JobTitle,
                    Position = _db.DepartmentsMembers.Where(dm => dm.UserId == approverId && dm.IsDeleted == false && dm.DepartmentId == departmentId).Select(d => d.Position).FirstOrDefault()
                }).FirstOrDefault();
                if (approver != null)
                {
                    approvers.Add(approver);
                }
            }

            return new Result<List<AccountApproversDTO>>(true, "Get all approvers successfully !", approvers);
        }
    }
}