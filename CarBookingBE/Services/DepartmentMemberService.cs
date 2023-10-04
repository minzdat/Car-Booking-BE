using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace CarBookingBE.Services
{
    public class DepartmentMemberService
    {
        MyDbContext _db = new MyDbContext();
        public Result<Pagination<DepartmentMemberDTO>> getAll(int page, int limit)
        {
            try
            {
                var dmsList = _db.DepartmentsMembers.Include(d => d.User).Include(d => d.Department)
                    .Where(dm => dm.IsDeleted == false)
                    .Select(d => new DepartmentMemberDTO
                    {
                        Id = d.Id,
                        Position = d.Position,
                        User = new AccountDTO
                        {
                            Id = d.User.Id,
                            Email = d.User.Email,
                            Username = d.User.Username,
                            FirstName = d.User.FirstName,
                            LastName = d.User.LastName,
                            JobTitle = d.User.JobTitle
                        },
                        Department = new DepartmentDTO
                        {
                            Id = d.Department.Id,
                            Name = d.Department.Name
                        }
                    })
                    .OrderBy(dm => dm.Id)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToList();
                if (!dmsList.Any())
                {
                    return new Result<Pagination<DepartmentMemberDTO>>(false, "There's no data !", new Pagination<DepartmentMemberDTO>());
                }
                var dPagination = new Pagination<DepartmentMemberDTO>
                {
                    PerPage = limit,
                    CurrentPage = page,
                    TotalPage = (dmsList.Count + limit - 1) / limit,
                    ListData = dmsList
                };
                return new Result<Pagination<DepartmentMemberDTO>>(true, "Get all data successfully !", dPagination);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Pagination<DepartmentMemberDTO>>(false, "Internal error !");
            }
        }
        public Result<List<DepartmentMemberDTO>> getByDepartmentId(string departmentId)
        {
            try
            {
                if (departmentId == null)
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "Missing id of department !", new List<DepartmentMemberDTO>());
                }
                var dId = Guid.Parse(departmentId);
                var pList = _db.DepartmentsMembers
                    .Include(d => d.User).Include(d => d.Department)
                    .Where(d => d.IsDeleted == false)
                    .Select(d => new DepartmentMemberDTO
                    {
                        Id = d.Id,
                        Position = d.Position,
                        User = new AccountDTO
                        {
                            Id = d.User.Id,
                            Username = d.User.Username,
                            Email = d.User.Email,
                            FirstName = d.User.FirstName,
                            LastName = d.User.LastName,
                            JobTitle = d.User.JobTitle
                        },
                        Department = new DepartmentDTO
                        {
                            Id = d.Department.Id,
                            Name = d.Department.Name
                        }
                    })
                    .Where(d => d.Department.Id == dId)
                    .ToList();

                if (!pList.Any())
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "There's no data with this department !", new List<DepartmentMemberDTO>());
                }
                return new Result<List<DepartmentMemberDTO>>(true, $"Get all data in the department successfully)", pList);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<DepartmentMemberDTO>>(false, "Internal error !", new List<DepartmentMemberDTO>());
            }
        }
        public Result<List<DepartmentMemberDTO>> getByPositionWithDeparmentId(string departmentId, string position)
        {
            try
            {
                if (departmentId == null)
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "Missing id of department !", new List<DepartmentMemberDTO>());
                }
                else if(position == null)
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "Missing position field !", new List<DepartmentMemberDTO>());
                }
                var dId = Guid.Parse(departmentId);
                var pList = _db.DepartmentsMembers
                    .Include(d => d.User).Include(d => d.Department)
                    .Where(d => d.IsDeleted == false && d.Position.Contains(position))
                    .Select(d => new DepartmentMemberDTO
                    {
                        Id = d.Id,
                        Position = d.Position,
                        User = new AccountDTO
                        {
                            Id = d.User.Id,
                            Username = d.User.Username,
                            Email = d.User.Email,
                            FirstName = d.User.FirstName,
                            LastName = d.User.LastName,
                            JobTitle = d.User.JobTitle
                        },
                        Department = new DepartmentDTO
                        {
                            Id = d.Department.Id,
                            Name = d.Department.Name
                        }
                    })
                    .Where(d => d.Department.Id == dId)
                    .ToList();

                if (!pList.Any())
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "There's no data with this position !", new List<DepartmentMemberDTO>());
                }
                return new Result<List<DepartmentMemberDTO>>(true, $"Get all data in the department successfully (position = {position.ToLower()})", pList);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<DepartmentMemberDTO>>(false, "Internal error !", new List<DepartmentMemberDTO>());
            }
        }
        public Result<DepartmentMember> addDepartmentMember(DepartmentMember newDM)
        {
            try
            {
                if (newDM == null || newDM.UserId == null || newDM.DepartmentId == null)
                {
                    return new Result<DepartmentMember>(false, "Missing parameter(s) !");
                }
                /*var reusable = _db.DepartmentsMembers
                    .Where(d => d.IsDeleted == true && d.UserId == newDM.UserId &&
                    d.DepartmentId == newDM.DepartmentId && d.Position == newDM.Position)
                    .FirstOrDefault();
                if (reusable != null)
                {
                    reusable.IsDeleted = false;
                    _db.SaveChanges();
                    return new Result<DepartmentMember>(true, "Add role for user successfully !", reusable);
                }*/
                var checkExist = _db.DepartmentsMembers
                    .Where(d => d.IsDeleted == false && d.UserId == newDM.UserId && d.DepartmentId == newDM.DepartmentId)
                    .FirstOrDefault();
                if (checkExist != null)
                {
                    return new Result<DepartmentMember>(false, "This employee's already existed in the department !");
                }
                newDM.Position = "Employee";
                _db.DepartmentsMembers.Add(newDM);
                _db.SaveChanges();
                return new Result<DepartmentMember>(true, "Add data successfully !", newDM);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<DepartmentMember>(false, "Internal error !");
            }
        }
        public Result<DepartmentMember> getDepartmentMember(string id)
        {
            try
            {
                var dm = _db.DepartmentsMembers.Find(Guid.Parse(id));
                if (dm == null || dm.IsDeleted == true)
                {
                    return new Result<DepartmentMember>(false, "Data (with input id) does not exist !");
                }
                return new Result<DepartmentMember>(true, "Get data successfully !", dm);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Result<DepartmentMember>(false, "Internal error !");
            }
        }
        public Result<DepartmentMember> editDepartmentMember(string id, DepartmentMember dmUpdate)
        {
            try
            {
                var dmTarget = _db.DepartmentsMembers.Find(Guid.Parse(id));
                if (dmTarget == null || dmTarget.IsDeleted == true)
                {
                    return new Result<DepartmentMember>(false, "Data (with input id) does not exist !");
                }
                if (dmUpdate.UserId != null) dmUpdate.UserId = dmUpdate.UserId;
                if (dmUpdate.DepartmentId != null) dmUpdate.DepartmentId = dmUpdate.DepartmentId;
                //if (dmUpdate.Position != null) dmUpdate.Position = dmUpdate.Position;
                
                _db.SaveChanges();
                return new Result<DepartmentMember>(true, "Edit data successfully !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Result<DepartmentMember>(false, "Internal error !");
            }
        }
        public Result<List<string>> getAllSupervisors(string did)
        {
            try
            {
                var dId = Guid.Parse(did);
                var data = _db.DepartmentsMembers.Where(d => d.Position.Contains("Supervisor") && d.DepartmentId == dId && d.IsDeleted == false)
                    .Select(d => d.UserId.ToString()).ToList();
                if (!data.Any())
                {
                    return new Result<List<string>>(false, "There's no data !");
                }
                return new Result<List<string>>(true, "Get supervisors successfully !", data);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<string>>(false, "Internal error !");
            }
        }
        public Result<string> getManagerByDepartment(string did)
        {
            var dId = Guid.Parse(did);
            var data = _db.DepartmentsMembers.FirstOrDefault(d => d.DepartmentId == dId && d.IsDeleted == false && d.Position.Contains("Manager"));
            if(data == null)
            {
                return new Result<string>(false, "There's no data !");
            }
            return new Result<string>(false, "Get manager successfully !", data.UserId.ToString());
        }
        public Result<DepartmentMember> deleteDepartmentMember(string id)
        {
            try
            {
                var del = _db.DepartmentsMembers.Find(Guid.Parse(id));
                if (del != null || del.IsDeleted == true)
                {
                    return new Result<DepartmentMember>(false, "Data (with input id) does not exist !");
                }
                del.IsDeleted = true;
                _db.SaveChanges();
                return new Result<DepartmentMember>(true, "Delete data successfully !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Result<DepartmentMember>(false, "Internal error !");
            }
        }

        public Result<List<string>> getDepartmentsByUserId(string userId)
        {
            try
            {
                var uId = Guid.Parse(userId);
                var departmentsById = new List<string>();
                var urs = _db.DepartmentsMembers.Where(ur => ur.UserId == uId).ToList();
                foreach (var item in urs)
                {
                    departmentsById.Add(item.DepartmentId.ToString());
                }
                return new Result<List<string>>(true, "Get roles by userId successfully !", departmentsById);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<string>>(false, "Get roles by userId failed !", new List<string>());
            }
        }
        //api for admin page
        public Result<string[]> addDepartmentMembers(string strUserId, string[] addDMList)
        {
            var userId = Guid.Parse(strUserId);
            var checkExistUser = _db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Id == userId);
            if (checkExistUser == null)
            {
                return new Result<string[]>(false, "User does not exist !");
            }
            if (addDMList == null || addDMList.Length == 0)
            {
                return new Result<string[]>(false, "Empty input departments list !");
            }
            var rs = getDepartmentsByUserId(strUserId);
            if (!rs.Success)
            {
                return new Result<string[]>(false, rs.Message);
            }
            foreach (var strDepartmentId in rs.Data)
            {
                if (!addDMList.Contains(strDepartmentId))
                {
                    deleteDepartmentRoleById(strUserId, strDepartmentId);
                }
            }
            foreach (var strDepartmentId in addDMList)
            {
                var departmentId = Guid.Parse(strDepartmentId);
                var role = _db.DepartmentsMembers.FirstOrDefault(u => u.IsDeleted == false && u.DepartmentId == departmentId && u.UserId == userId);
                if (role == null)
                {
                    var addSuccess = addDepartmentMember(new DepartmentMember { UserId = userId, DepartmentId = departmentId });
                    if (!addSuccess.Success) break;
                }
            }
            return new Result<string[]>(true, "Add roles for user successfully !", addDMList);
        }

        public Result<string> deleteDepartmentRoleById(string strUserId, string strDepartmentId)
        {
            var uId = Guid.Parse(strUserId);
            var dId = Guid.Parse(strDepartmentId);
            if (uId == null)
            {
                return new Result<string>(false, "Missing or invalid id of user !");
            }
            if (dId == null)
            {
                return new Result<string>(false, "Missing or invalid id of department !");
            }
            var existUser = _db.Users.FirstOrDefault(u => u.Id == uId);
            if (existUser == null)
            {
                return new Result<string>(false, "User with input id does not exist");
            }
            var existRole = _db.Departments.FirstOrDefault(r => r.Id == dId);
            if (existRole == null)
            {
                return new Result<string>(false, "Department with input title does not exist !");
            }
            var delData = _db.DepartmentsMembers.FirstOrDefault(ur => ur.DepartmentId == dId && ur.UserId == uId && ur.IsDeleted == false);
            if (delData != null)
            {
                _db.DepartmentsMembers.Remove(delData);
                _db.SaveChanges();
                return new Result<string>(true, "Delete a department member successfully !");
            }
            return new Result<string>(false, "Not found data to delete !");
        }
    }
}