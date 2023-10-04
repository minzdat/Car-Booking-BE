using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CarBookingBE.Services
{
    public class DepartmentService
    {
        MyDbContext _db = new MyDbContext();
        public Result<Pagination<Department>> getAll(int page, int limit)
        {
            try
            {
                var initDepartments = _db.Departments.Where(d => d.IsDeleted == false)
                .OrderByDescending(user => user.Code);
                var departments = initDepartments
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
                if(!departments.Any())
                {
                    return new Result<Pagination<Department>>(false, "There's no data !");
                }
                var dPagination = new Pagination<Department>
                {
                    PerPage = limit,
                    CurrentPage = page,
                    TotalPage = (initDepartments.ToList().Count + limit - 1) / limit,
                    ListData = departments
                };
                return new Result<Pagination<Department>>(true, "Get all departments successfully !", dPagination);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return new Result<Pagination<Department>>(false, "Internal error !");
            }
        }
        public Result<List<Department>> getAll()
        {
            try
            {
                var departments = _db.Departments.Where(d => d.IsDeleted == false).ToList();
                if (!departments.Any())
                {
                    return new Result<List<Department>>(false, "There's no data !");
                }
                return new Result<List<Department>>(true, "Get all departments successfully !", departments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new Result<List<Department>>(false, "Internal error !");
            }
        }
        public Result<Department> addDepartment(Department department)
        {
            try
            {
                if (department == null || department.Name == null || department.ContactInfo == null || department.Code == null || department.Description == null)
                {
                    return new Result<Department>(false, "Missing parameter(s) !");
                }
                /*var reusable = _db.Departments
                    .Where(d => d.IsDeleted == true && d.Name == department.Name && d.ContactInfo == department.ContactInfo &&
                    d.Code == department.Code && d.Description == department.Description)
                    .FirstOrDefault();
                if (reusable != null)
                {
                    reusable.IsDeleted = false;
                    _db.SaveChanges();
                    return new Result<Department>(true, "Add role for user successfully !", reusable);
                }*/
                _db.Departments.Add(department);
                _db.SaveChanges();
                return new Result<Department>(true, "Add department succesfully !", department);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Department>(false, "Internal error !");
            }
        }
        public Result<Department> getDepartment(string id)
        {
            try
            {
                var department = _db.Departments.Find(Guid.Parse(id));
                if(department == null || department.IsDeleted == true)
                {
                    return new Result<Department>(false, "Department does not exist !");
                }
                return new Result<Department>(true, "Get department successfully !", department);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e);
                return new Result<Department>(false, "Internal error !");
            }
        }
        public Result<Department> editDepartment(string id, PositionDepartmentDTO dUpdate)
        {
            try
            {
                var did = Guid.Parse(id);
                var dTarget = _db.Departments.FirstOrDefault(d => d.IsDeleted == false && d.Id == did);
                if(dTarget == null)
                {
                    return new Result<Department>(false, "Department does not exist !");
                }
                
                if(dUpdate.Name != null) dTarget.Name = dUpdate.Name;
                if(dUpdate.ContactInfo != null) dTarget.ContactInfo = dUpdate.ContactInfo;
                if(dUpdate.Code != null) dTarget.Code = dUpdate.Code;
                if(dUpdate.UnderDepartment != null)
                {
                    dTarget.UnderDepartment = dUpdate.UnderDepartment;
                }
                else
                {
                    dTarget.UnderDepartment = null;
                }
                if(dUpdate.Description != null) dTarget.Description = dUpdate.Description;

                var allEmployees = _db.DepartmentsMembers.Where(m => m.DepartmentId == dUpdate.Id).ToList();
                var oldManager = _db.DepartmentsMembers.FirstOrDefault(d => d.Position.Contains("Manager") && d.DepartmentId == dUpdate.Id);
                if (dUpdate.Manager != null)
                {
                    //check and remove old manager
                    var newManagerId = Guid.Parse(dUpdate.Manager);
                    var hasNewManager = oldManager == null || (newManagerId != null && oldManager.UserId != newManagerId);
                    if (hasNewManager)
                    {
                        foreach (var item in allEmployees)
                        {
                            var emPos = item.Position.Split(',').ToList();
                            if (emPos.Count > 0 && emPos.Contains("Manager"))
                            {
                                emPos.Remove("Manager");
                            }
                            item.Position = emPos.Count > 0 ? item.Position = string.Join(",", emPos) : "Employee";
                        }
                    }
                
                    var mid = Guid.Parse(dUpdate.Manager);
                    var manager = allEmployees.FirstOrDefault(e => e.UserId == mid);
                    if (manager != null)
                    {
                        var listPos = manager.Position.Split(',').ToList();
                        if(dUpdate.ManEm)
                        {
                            if(!listPos.Contains("Employee")) listPos.Add("Employee");
                        }
                        else
                        {
                            if (listPos.Contains("Employee")) listPos.Remove("Employee");
                        }
                        if (!listPos.Contains("Manager"))
                        {
                            listPos.Add("Manager");
                        }
                        manager.Position = string.Join(",", listPos);
                    }
                }
                if(dUpdate.Supervisors != null)
                {
                    //remove all old supervisors
                    foreach (var em in allEmployees)
                    {
                        var listPosRemove = em.Position.Split(',').ToList();
                        if(listPosRemove.Contains("Supervisor"))
                        {
                            listPosRemove.Remove("Supervisor");
                        }
                        em.Position = listPosRemove.Count > 0 ? string.Join(",", listPosRemove) : "Employee";
                    }
                    //edit new
                    foreach (var item in dUpdate.Supervisors)
                    {
                        var uid = Guid.Parse(item);
                        var supervisor = allEmployees.FirstOrDefault(s => s.UserId == uid);
                        if (supervisor != null)
                        {
                            var listPos = supervisor.Position.Split(',').ToList();
                            if (dUpdate.SupEm)
                            {
                                if (!listPos.Contains("Employee")) listPos.Add("Employee");
                            }
                            else
                            {
                                if (listPos.Contains("Employee")) listPos.Remove("Employee");
                            }
                            if (!listPos.Contains("Supervisor"))
                            {
                                listPos.Add("Supervisor");
                            }
                            supervisor.Position = string.Join(",", listPos);
                        }
                    }
                }
                _db.SaveChanges();
                return new Result<Department>(true, "Edit department successfully !");
            }
            catch(Exception e)
            {
                Trace.WriteLine(e);
                return new Result<Department>(false, "Internal error !");
            }
        }
        public Result<Department> deleteDepartment(string id)
        {
            try
            {
                var did = Guid.Parse(id);
                var del = _db.Departments.FirstOrDefault(d => d.Id == did && d.IsDeleted == false);
                if(del == null)
                {
                    return new Result<Department>(false, "Department does not exist !");
                }
                var deleteDepartmentMember = _db.DepartmentsMembers.Where(d => d.IsDeleted == false && d.DepartmentId == del.Id).ToList();
                if(deleteDepartmentMember.Any() )
                {
                    return new Result<Department>(false, "Cannot delete, this department has some related data in other places !");
                }
                /*foreach (var item in deleteDepartmentMember)
                {
                    item.IsDeleted = true;
                }*/
                //del.IsDeleted = true;
                _db.Departments.Remove(del);
                _db.SaveChanges();
                return new Result<Department>(true, "Delete department successfully !");
            }
            catch(Exception e)
            {
                Trace.WriteLine(e);
                return new Result<Department>(false, "Internal error !");
            }
        }
    }
}