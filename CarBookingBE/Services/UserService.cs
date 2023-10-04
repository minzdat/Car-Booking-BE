using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace CarBookingBE.Services
{
    public class UserService
    {
        MyDbContext _db = new MyDbContext();
        HandlePassword hp = new HandlePassword();
        TokenProps jwt = new TokenProps();
        FileService fileService = new FileService();
        RoleConstants roleConstants = new RoleConstants();
        UserRoleService urService = new UserRoleService();
        DepartmentMemberService dms = new DepartmentMemberService();
        UtilMethods util = new UtilMethods();
        public Result<LoginReturnDTO> loginService(LoginDTO user)
        {
            if (user == null || user.Username == null || user.Password == null)
            {
                return new Result<LoginReturnDTO>(false, "Missing required parameter !");
            }
            else if (user.Username.Trim().Length == 0)
            {
                return new Result<LoginReturnDTO>(false, "Username is required !");
            }
            else if (user.Password.Trim().Length == 0)
            {
                return new Result<LoginReturnDTO>(false, "Password is required !");
            }
            else
            {
                string hash = hp.HashPassword(user.Password);
                var userInfo = _db.Users
                    .Where(ur => ur.IsDeleted == false && ur.Username == user.Username && ur.Password == hash)
                    .Select(ur => new AccountLoginReturnDTO
                    {
                        Id = ur.Id,
                        Username = ur.Username,
                        Email = ur.Email,
                        FirstName = ur.FirstName,
                        LastName = ur.LastName,
                        AvatarPath = ur.AvatarPath,
                    })
                    .FirstOrDefault();
                //var validUser = _db.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == hash);
                if (userInfo != null)
                {
                    string secretKey = jwt.secretKey;
                    string issuer = jwt.issuer;
                    string audience = jwt.audience;
                    int expirationMinutes = jwt.expirationMinutes;
                    Claim[] customClaims = new Claim[]
                    {
                        new Claim("CurId", userInfo.Id.ToString()),
                        new Claim("Username", user.Username),
                        new Claim("Password", user.Password)
                    };
                    string jwtToken = jwt.GenerateJwtToken(secretKey, issuer, audience, expirationMinutes, customClaims);
                    //bool check = jwt.VerifyJwtToken(jwtToken, secretKey);
                    var userRole = _db.UserRoles
                        .Where(ur => ur.IsDeleted == false && ur.UserId == userInfo.Id)
                        .Select(ur => new UserRolesDTO
                        {
                            Role = new RoleDTO
                            {
                                Id = ur.Role.Id,
                                Title = ur.Role.Title
                            }
                        })
                        .ToList();

                    return new Result<LoginReturnDTO>(true, "Login successfully !", new LoginReturnDTO(userInfo, jwtToken, userRole));
                }
                else
                {
                    return new Result<LoginReturnDTO>(false, "Username or password is not correct !");
                }
            }
        }
        public Result<Account> registerService(Guid curId, HttpPostedFile file, Account user, string roleId)
        {
            try
            {
                var rId = int.Parse(roleId);
                if (user == null)
                {
                    return new Result<Account>(false, "Missing all parameters !");
                }
                else if (user.Email == null || user.Email.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Email is required !");
                }
                else if (!util.IsValidEmail(user.Email))
                {
                    return new Result<Account>(false, "Wrong format of email !");
                }
                else if (_db.Users.FirstOrDefault(u => u.Email == user.Email) != null)
                {
                    return new Result<Account>(false, "This email has existed !");
                }
                else if (user.Password == null || user.Password.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Password is required !");
                }
                else if (user.FirstName == null || user.FirstName.Trim().Length == 0)
                {
                    return new Result<Account>(false, "First name is required !");
                }
                else if (user.LastName == null || user.LastName.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Last name is required !");
                }
                else if (user.Sex.ToString() == null || user.Sex.ToString().Trim().Length == 0)
                {
                    return new Result<Account>(false, "Sex is required !");
                }
                else if (user.EmployeeNumber == null || user.EmployeeNumber.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Employee number is required !");
                }
                else if (_db.Users.FirstOrDefault(u => u.EmployeeNumber == user.EmployeeNumber) != null)
                {
                    return new Result<Account>(false, "This employee number has existed !");
                }
                else if (roleId == null || roleId.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Missing role id of user !");
                }
                else if (_db.Roles.Where(role => role.IsDeleted == false && role.Id == rId).FirstOrDefault() == null)
                {
                    return new Result<Account>(false, "Role with input id does not exist !");
                }
                else if (user.JobTitle == null || user.JobTitle.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Missing job title of user !");
                }
                else
                {
                    string flag = "";
                    if (file != null && file.ContentType != null && file.FileName.Count() != 0)
                    {
                        var uploadResult = fileService.uploadAvatar(curId.ToString(), file);
                        if (!uploadResult.Success)
                        {
                            return new Result<Account>(false, uploadResult.Message);
                        }
                        flag = uploadResult.Data;
                    }
                    Account newUser = new Account();
                    newUser.Email = user.Email;
                    newUser.Username = user.Email;
                    newUser.Password = hp.HashPassword(user.Password);
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Sex = user.Sex;
                    newUser.EmployeeNumber = user.EmployeeNumber;
                    newUser.IsDeleted = false;
                    newUser.Created = DateTime.Now;
                    if (flag.Length > 0)
                    {
                        newUser.AvatarPath = flag;
                    }
                    else
                    {
                        newUser.AvatarPath = "Files/Avatar/default-user-profile.png";
                    }
                    _db.Users.Add(newUser);

                    AccountRole ar = new AccountRole
                    {
                        UserId = user.Id,
                        RoleId = rId
                    };
                    _db.SaveChanges();
                    var addUserRoleResult = urService.addUserRole(ar);
                    if (!addUserRoleResult.Success)
                    {
                        return new Result<Account>(false, addUserRoleResult.Message);
                    }

                    return new Result<Account>(true, "Register new user successfully !", newUser);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Account>(false, "Internal error !");
            }
        }
        public Result<Pagination<Account>> getUsersService(int page, int limit)
        {
            try
            {
                var initUsers = _db.Users.Where(u => u.IsDeleted == false)
                .OrderByDescending(user => user.Created);
                var users = initUsers
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

                var usersPagination = new Pagination<Account> { 
                    PerPage = limit,
                    CurrentPage = page,
                    TotalPage = (initUsers.ToList().Count + limit - 1) / limit,
                    ListData = users
                };
                if (users == null)
                {
                    return new Result<Pagination<Account>>(true, "There's no data !", new Pagination<Account>());
                }
                return new Result<Pagination<Account>>(true, "Get all users successfully !", usersPagination);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Pagination<Account>>(false, "Internal error !");
            }
        }

        public Result<Account> getProfileService(string id)
        {
            try
            {
                var userId = Guid.Parse(id);
                var user = _db.Users.Include(u => u.UserRoles)
                    .FirstOrDefault(u => u.IsDeleted == false && u.Id == userId);
                if (user == null)
                {
                    return new Result<Account>(false, "User does not exist");
                }
                return new Result<Account>(true, "Get user profile successfully !", user);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Account>(false, "Internal error !");
            }
        }

        public Result<Account> editProfileWithPostFile(Guid curId, HttpPostedFile postedFile, string userIdString, NameValueCollection updateUser)
        {
            try
            {
                var userId = Guid.Parse(userIdString);
                var user = _db.Users.Find(userId);
                if (user == null || user.IsDeleted == true)
                {
                    return new Result<Account>(false, "User do not exist !");
                }

                //start edit fields
                if (postedFile != null)
                {
                    var rs = fileService.uploadAvatar(curId.ToString(), postedFile);
                    if (!rs.Success)
                    {
                        return new Result<Account>(false, rs.Message);
                    }
                    user.AvatarPath = rs.Data;
                }

                bool isAdmin = util.userHasRole(curId, roleConstants.ADMIN);
                if (isAdmin)
                {
                    //Password, IsDeleted
                    if (updateUser["Birthday"] != null && util.stringValid(updateUser["Birthday"]))
                    {
                        try
                        {
                            user.Birthday = DateTime.ParseExact(updateUser["Birthday"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (user.Birthday > DateTime.Now)
                            {
                                return new Result<Account>(false, "Birthday cannot exceed time in the future");
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Birthday");
                        }
                    }
                    if (updateUser["StartingDateOfficial"] != null && util.stringValid(updateUser["StartingDateOfficial"]))
                    {
                        try
                        {
                            user.StartingDateOfficial = DateTime.ParseExact(updateUser["StartingDateOfficial"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Starting Date Official");
                        }
                    }
                    if (updateUser["LeavingDate"] != null && util.stringValid(updateUser["LeavingDate"]))
                    {
                        try
                        {
                            user.LeavingDate = DateTime.ParseExact(updateUser["LeavingDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Leaving Date");
                        }
                    }
                    if (updateUser["StartDateMaternityLeave"] != null && util.stringValid(updateUser["StartDateMaternityLeave"]))
                    {
                        try
                        {
                            user.StartDateMaternityLeave = DateTime.ParseExact(updateUser["StartDateMaternityLeave"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Start Date Maternity Leave");
                        }
                    }

                    if (updateUser["Username"] != null && util.stringValid(updateUser["Username"]) && !user.Username.Equals(updateUser["Username"]))
                    {
                        var username = updateUser["Username"];
                        var check = _db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Username == username);
                        if (check != null)
                        {
                            return new Result<Account>(false, "This username's already existed !");
                        }
                        user.Username = updateUser["Username"];
                    }
                    if (updateUser["Email"] != null && util.stringValid(updateUser["Email"]) && !user.Email.Equals(updateUser["Email"]))
                    {
                        var email = updateUser["Email"];
                        var check = _db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Email == email);
                        if (check != null)
                        {
                            return new Result<Account>(false, "This username's already existed !");
                        }
                        if (!util.IsValidEmail(updateUser["Email"]))
                        {
                            return new Result<Account>(false, "Wrong format of email !");
                        }
                        user.Email = updateUser["Email"];
                    }
                    if (updateUser["EmployeeNumber"] != null && util.stringValid(updateUser["EmployeeNumber"]) && !user.EmployeeNumber.Equals(updateUser["EmployeeNumber"]))
                    {
                        var emNum = updateUser["EmployeeNumber"];
                        var check = _db.Users.FirstOrDefault(u => u.IsDeleted == false && u.EmployeeNumber == emNum);
                        if (check != null)
                        {
                            return new Result<Account>(false, "Duplicate employee number !");
                        }
                        user.EmployeeNumber = updateUser["EmployeeNumber"];
                    }
                    if (updateUser["FirstName"] != null && util.stringValid(updateUser["FirstName"])) user.FirstName = updateUser["FirstName"];
                    if (updateUser["LastName"] != null && util.stringValid(updateUser["LastName"])) user.LastName = updateUser["LastName"];
                    if (updateUser["Sex"] != null && util.stringValid(updateUser["Sex"]))
                    {
                        try
                        {
                            user.Sex = bool.Parse(updateUser["Sex"]);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Type of sex is invalid !");
                        }
                    }
                    if (updateUser["JobTitle"] != null && util.stringValid(updateUser["JobTitle"])) user.JobTitle = updateUser["JobTitle"];
                    if (updateUser["Company"] != null && util.stringValid(updateUser["Company"])) user.Company = updateUser["Company"];
                    if (updateUser["Unit"] != null && util.stringValid(updateUser["Unit"])) user.Unit = updateUser["Unit"];
                    if (updateUser["Function"] != null && util.stringValid(updateUser["Function"])) user.Function = updateUser["Function"];
                    if (updateUser["SectionsOrTeam"] != null && util.stringValid(updateUser["SectionsOrTeam"])) user.SectionsOrTeam = updateUser["SectionsOrTeam"];
                    if (updateUser["Groups"] != null && util.stringValid(updateUser["Groups"])) user.Groups = updateUser["Groups"];
                    if (updateUser["OfficeLocation"] != null && util.stringValid(updateUser["OfficeLocation"])) user.OfficeLocation = updateUser["OfficeLocation"];
                    if (updateUser["LineManager"] != null && util.stringValid(updateUser["LineManager"])) user.LineManager = updateUser["LineManager"];
                    if (updateUser["BelongToDepartments"] != null && util.stringValid(updateUser["BelongToDepartments"])) user.BelongToDepartments = updateUser["BelongToDepartments"];

                }

                // user can edit
                if (updateUser["AvatarPath"] != null && util.stringValid(updateUser["AvatarPath"]))
                {
                    user.AvatarPath = updateUser["AvatarPath"];
                }
                if (updateUser["DateOfIdCard"] != null && util.stringValid(updateUser["DateOfIdCard"]))
                {
                    try
                    {
                        user.DateOfIdCard = DateTime.ParseExact(updateUser["DateOfIdCard"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        if (user.DateOfIdCard > DateTime.Now)
                        {
                            return new Result<Account>(false, "Date Of Id Card cannot exceed time in the future");
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Date Of Id Card");
                    }
                }

                if (updateUser["StartingDate"] != null && util.stringValid(updateUser["StartingDate"]))
                {
                    try
                    {
                        user.StartingDate = DateTime.ParseExact(updateUser["StartingDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        if (user.StartingDate > DateTime.Now)
                        {
                            return new Result<Account>(false, "Starting Date (Interviewed date) cannot exceed time in the future");
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Starting Date (Interviewed date)");
                    }
                }
                user.Rank = updateUser["Rank"];
                user.EmployeeType = updateUser["EmployeeType"];
                user.Rights = updateUser["Rights"];
                user.Nation = updateUser["Nation"];
                user.Phone = updateUser["Phone"];
                user.IdCardNumber = updateUser["IdCardNumber"];
                user.PlaceOfIdCard = updateUser["PlaceOfIdCard"];
                user.HealthInsurance = updateUser["HealthInsurance"];
                user.Note = updateUser["Note"];
                user.AcademicLevel = updateUser["AcademicLevel"];
                user.Qualification = updateUser["Qualification"];
                user.BusinessPhone = updateUser["BusinessPhone"];
                user.HomePhone = updateUser["HomePhone"];
                user.PersonalEmail = updateUser["PersonalEmail"];
                user.BankName = updateUser["BankName"];
                user.BankBranchNumber = updateUser["BankBranchNumber"];
                user.BankBranchName = updateUser["BankBranchName"];
                user.BankAccountNumber = updateUser["BankAccountNumber"];
                user.BankAccountName = updateUser["BankAccountName"];
                user.Street = updateUser["Street"];
                user.FlatNumber = updateUser["FlatNumber"];
                user.City = updateUser["City"];
                user.Province = updateUser["Province"];
                user.PostalCode = updateUser["PostalCode"];
                user.Country = updateUser["Country"];
                user.MartialStatus = updateUser["MartialStatus"];
                user.ContactName = updateUser["ContactName"];
                user.Relationship = updateUser["Relationship"];
                user.PhoneR = updateUser["PhoneR"];
                user.StreetR = updateUser["StreetR"];
                user.FlatNumberR = updateUser["FlatNumberR"];
                user.CityR = updateUser["CityR"];
                user.ProvinceR = updateUser["ProvinceR"];
                user.PostalCodeR = updateUser["PostalCodeR"];
                user.CountryR = updateUser["CountryR"];
                
                /* if (updateUser["Rank"] != null && util.stringValid(updateUser["Rank"])) user.Rank = updateUser["Rank"];
                if (updateUser["EmployeeType"] != null && util.stringValid(updateUser["EmployeeType"])) user.EmployeeType = updateUser["EmployeeType"];
                if (updateUser["Rights"] != null && util.stringValid(updateUser["Rights"])) user.Rights = updateUser["Rights"];
                if (updateUser["Nation"] != null && util.stringValid(updateUser["Nation"])) user.Nation = updateUser["Nation"];
                if (updateUser["Phone"] != null && util.stringValid(updateUser["Phone"])) user.Phone = updateUser["Phone"];
                if (updateUser["IdCardNumber"] != null && util.stringValid(updateUser["IdCardNumber"])) user.IdCardNumber = updateUser["IdCardNumber"];
                if (updateUser["PlaceOfIdCard"] != null && util.stringValid(updateUser["PlaceOfIdCard"])) user.PlaceOfIdCard = updateUser["PlaceOfIdCard"];
                if (updateUser["HealthInsurance"] != null && util.stringValid(updateUser["HealthInsurance"])) user.HealthInsurance = updateUser["HealthInsurance"];
                if (updateUser["Note"] != null && util.stringValid(updateUser["Note"])) user.Note = updateUser["Note"];
                if (updateUser["AcademicLevel"] != null && util.stringValid(updateUser["AcademicLevel"])) user.AcademicLevel = updateUser["AcademicLevel"];
                if (updateUser["Qualification"] != null && util.stringValid(updateUser["Qualification"])) user.Qualification = updateUser["Qualification"];
                if (updateUser["BusinessPhone"] != null && util.stringValid(updateUser["BusinessPhone"])) user.BusinessPhone = updateUser["BusinessPhone"];
                if (updateUser["HomePhone"] != null && util.stringValid(updateUser["HomePhone"])) user.HomePhone = updateUser["HomePhone"];
                if (updateUser["PersonalEmail"] != null && util.stringValid(updateUser["PersonalEmail"])) user.PersonalEmail = updateUser["PersonalEmail"];
                if (updateUser["BankName"] != null && util.stringValid(updateUser["BankName"])) user.BankName = updateUser["BankName"];
                if (updateUser["BankBranchNumber"] != null && util.stringValid(updateUser["BankBranchNumber"])) user.BankBranchNumber = updateUser["BankBranchNumber"];
                if (updateUser["BankBranchName"] != null && util.stringValid(updateUser["BankBranchName"])) user.BankBranchName = updateUser["BankBranchName"];
                if (updateUser["BankAccountNumber"] != null && util.stringValid(updateUser["BankAccountNumber"])) user.BankAccountNumber = updateUser["BankAccountNumber"];
                if (updateUser["BankAccountName"] != null && util.stringValid(updateUser["BankAccountName"])) user.BankAccountName = updateUser["BankAccountName"];
                if (updateUser["Street"] != null && util.stringValid(updateUser["Street"])) user.Street = updateUser["Street"];
                if (updateUser["FlatNumber"] != null && util.stringValid(updateUser["FlatNumber"])) user.FlatNumber = updateUser["FlatNumber"];
                if (updateUser["City"] != null && util.stringValid(updateUser["City"])) user.City = updateUser["City"];
                if (updateUser["Province"] != null && util.stringValid(updateUser["Province"])) user.Province = updateUser["Province"];
                if (updateUser["PostalCode"] != null && util.stringValid(updateUser["PostalCode"])) user.PostalCode = updateUser["PostalCode"];
                if (updateUser["Country"] != null && util.stringValid(updateUser["Country"])) user.Country = updateUser["Country"];
                if (updateUser["MartialStatus"] != null && util.stringValid(updateUser["MartialStatus"])) user.MartialStatus = updateUser["MartialStatus"];
                if (updateUser["ContactName"] != null && util.stringValid(updateUser["ContactName"])) user.ContactName = updateUser["ContactName"];
                if (updateUser["Relationship"] != null && util.stringValid(updateUser["Relationship"])) user.Relationship = updateUser["Relationship"];
                if (updateUser["PhoneR"] != null && util.stringValid(updateUser["PhoneR"])) user.PhoneR = updateUser["PhoneR"];
                if (updateUser["StreetR"] != null && util.stringValid(updateUser["StreetR"])) user.StreetR = updateUser["StreetR"];
                if (updateUser["FlatNumberR"] != null && util.stringValid(updateUser["FlatNumberR"])) user.FlatNumberR = updateUser["FlatNumberR"];
                if (updateUser["CityR"] != null && util.stringValid(updateUser["CityR"])) user.CityR = updateUser["CityR"];
                if (updateUser["ProvinceR"] != null && util.stringValid(updateUser["ProvinceR"])) user.ProvinceR = updateUser["ProvinceR"];
                if (updateUser["PostalCodeR"] != null && util.stringValid(updateUser["PostalCodeR"])) user.PostalCodeR = updateUser["PostalCodeR"];
                if (updateUser["CountryR"] != null && util.stringValid(updateUser["CountryR"])) user.CountryR = updateUser["CountryR"]; */
                //if (updateUser["Signature"] != null && util.stringValid(updateUser["Signature"])) user.Signature = updateUser["Signature"];
                user.Created = DateTime.Now;

                _db.SaveChanges();
                return new Result<Account>(true, "Edit profile successfully !", user);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return new Result<Account>(false, "Internal error !");
            }
        }

        //admin page
        public Result<Account> addUserService(AccountForAddDTO user)
        {
            try
            {
                if (user == null)
                {
                    return new Result<Account>(false, "Missing all parameters !");
                }
                else if (user.Roles == null || user.Roles.Length == 0)
                {
                    return new Result<Account>(false, "Empty list roles of user !");
                }
                else if (user.Email == null || user.Email.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Email is required !");
                }
                else if (_db.Users.FirstOrDefault(u => u.Email == user.Email) != null)
                {
                    return new Result<Account>(false, "This email has existed !");
                }
                else if (user.Password == null || user.Password.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Password is required !");
                }
                else if (user.FirstName == null || user.FirstName.Trim().Length == 0)
                {
                    return new Result<Account>(false, "First name is required !");
                }
                else if (user.LastName == null || user.LastName.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Last name is required !");
                }
                else if (user.Sex.ToString() == null || user.Sex.ToString().Trim().Length == 0)
                {
                    return new Result<Account>(false, "Sex is required !");
                }
                else if (user.EmployeeNumber == null || user.EmployeeNumber.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Employee number is required !");
                }
                else if (_db.Users.FirstOrDefault(u => u.EmployeeNumber == user.EmployeeNumber) != null)
                {
                    return new Result<Account>(false, "This employee number has existed !");
                }
                else
                {
                    Account newUser = new Account();
                    newUser.Email = user.Email;
                    newUser.Username = user.Email;
                    newUser.Password = hp.HashPassword(user.Password);
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Sex = user.Sex;
                    newUser.EmployeeNumber = user.EmployeeNumber;
                    //Hard code
                    newUser.IsDeleted = false;
                    newUser.Company = "Opus Solution";
                    newUser.OfficeLocation = "366 Nguyen Trai, Ward 8, District 5, Ho Chi Minh City";
                    newUser.Created = DateTime.Now;
                    //upload client side
                    if (user.AvatarPath != null && util.stringValid(user.AvatarPath))
                    {
                        newUser.AvatarPath = user.AvatarPath;
                    }
                    else
                    {
                        newUser.AvatarPath = "Files/Avatar/default-user-profile.png";
                    }
                    //Optional
                    if(user.Roles == null && user.Roles.Length == 0)
                    {
                        return new Result<Account>(false, "User roles list is empty !");
                    }
                    if (user.Departments == null && user.Departments.Length == 0)
                    {
                        return new Result<Account>(false, "User departments list is empty !");
                    }
                    if (user.Birthday != null)
                    {
                        try
                        {
                            newUser.Birthday = user.Birthday;
                            //user.Birthday = DateTime.ParseExact(newUser.Birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (newUser.Birthday > DateTime.Now)
                            {
                                return new Result<Account>(false, "Birthday cannot exceed time in the future");
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Birthday");
                        }
                    }
                    if (user.StartingDateOfficial != null)
                    {
                        try
                        {
                            newUser.StartingDateOfficial = user.StartingDateOfficial;
                            //user.StartingDateOfficial = DateTime.ParseExact(newUser.StartingDateOfficial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Starting Date Official");
                        }
                    }
                    if (user.LeavingDate != null)
                    {
                        try
                        {
                            newUser.LeavingDate = user.LeavingDate;
                            //newUser.LeavingDate = DateTime.ParseExact(newUser.LeavingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Leaving Date");
                        }
                    }
                    if (user.StartDateMaternityLeave != null)
                    {
                        try
                        {
                            newUser.StartDateMaternityLeave = user.StartDateMaternityLeave;
                            //newUser.StartDateMaternityLeave = DateTime.ParseExact(newUser.StartDateMaternityLeave, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Start Date Maternity Leave");
                        }
                    }

                    if (user.JobTitle != null && util.stringValid(user.JobTitle)) newUser.JobTitle = user.JobTitle;
                    if (user.Unit != null && util.stringValid(user.Unit)) newUser.Unit = user.Unit;
                    if (user.Function != null && util.stringValid(user.Function)) newUser.Function = user.Function;
                    if (user.SectionsOrTeam != null && util.stringValid(user.SectionsOrTeam)) newUser.SectionsOrTeam = user.SectionsOrTeam;
                    if (user.Groups != null && util.stringValid(user.Groups)) newUser.Groups = user.Groups;
                    if (user.LineManager != null && util.stringValid(user.LineManager)) newUser.LineManager = user.LineManager;
                    if (user.BelongToDepartments != null && util.stringValid(user.BelongToDepartments)) newUser.BelongToDepartments = user.BelongToDepartments;

                    if (user.DateOfIdCard != null)
                    {
                        try
                        {
                            newUser.DateOfIdCard = user.DateOfIdCard;
                            //newUser.DateOfIdCard = DateTime.ParseExact(newUser.DateOfIdCard, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (newUser.DateOfIdCard > DateTime.Now)
                            {
                                return new Result<Account>(false, "Date Of Id Card cannot exceed time in the future");
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Date Of Id Card");
                        }
                    }

                    if (user.StartingDate != null)
                    {
                        try
                        {
                            newUser.StartingDate = user.StartingDate;
                            //newUser.StartingDate = DateTime.ParseExact(newUser.StartingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (newUser.StartingDate > DateTime.Now)
                            {
                                return new Result<Account>(false, "Starting Date (Interviewed date) cannot exceed time in the future");
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Starting Date (Interviewed date)");
                        }
                    }
                    if (user.Rank != null && util.stringValid(user.Rank)) newUser.Rank = user.Rank;
                    if (user.EmployeeType != null && util.stringValid(user.EmployeeType)) newUser.EmployeeType = user.EmployeeType;
                    if (user.Rights != null && util.stringValid(user.Rights)) newUser.Rights = user.Rights;
                    if (user.Nation != null && util.stringValid(user.Nation)) newUser.Nation = user.Nation;
                    if (user.Phone != null && util.stringValid(user.Phone)) newUser.Phone = user.Phone;
                    if (user.IdCardNumber != null && util.stringValid(user.IdCardNumber)) newUser.IdCardNumber = user.IdCardNumber;
                    if (user.PlaceOfIdCard != null && util.stringValid(user.PlaceOfIdCard)) newUser.PlaceOfIdCard = user.PlaceOfIdCard;
                    if (user.HealthInsurance != null && util.stringValid(user.HealthInsurance)) newUser.HealthInsurance = user.HealthInsurance;
                    if (user.Note != null && util.stringValid(user.Note)) newUser.Note = user.Note;
                    if (user.AcademicLevel != null && util.stringValid(user.AcademicLevel)) newUser.AcademicLevel = user.AcademicLevel;
                    if (user.Qualification != null && util.stringValid(user.Qualification)) newUser.Qualification = user.Qualification;
                    if (user.BusinessPhone != null && util.stringValid(user.BusinessPhone)) newUser.BusinessPhone = user.BusinessPhone;
                    if (user.HomePhone != null && util.stringValid(user.HomePhone)) newUser.HomePhone = user.HomePhone;
                    if (user.PersonalEmail != null && util.stringValid(user.PersonalEmail)) newUser.PersonalEmail = user.PersonalEmail;
                    if (user.BankName != null && util.stringValid(user.BankName)) newUser.BankName = user.BankName;
                    if (user.BankBranchNumber != null && util.stringValid(user.BankBranchNumber)) newUser.BankBranchNumber = user.BankBranchNumber;
                    if (user.BankBranchName != null && util.stringValid(user.BankBranchName)) newUser.BankBranchName = user.BankBranchName;
                    if (user.BankAccountNumber != null && util.stringValid(user.BankAccountNumber)) newUser.BankAccountNumber = user.BankAccountNumber;
                    if (user.BankAccountName != null && util.stringValid(user.BankAccountName)) newUser.BankAccountName = user.BankAccountName;
                    if (user.Street != null && util.stringValid(user.Street)) newUser.Street = user.Street;
                    if (user.FlatNumber != null && util.stringValid(user.FlatNumber)) newUser.FlatNumber = user.FlatNumber;
                    if (user.City != null && util.stringValid(user.City)) newUser.City = user.City;
                    if (user.Province != null && util.stringValid(user.Province)) newUser.Province = user.Province;
                    if (user.PostalCode != null && util.stringValid(user.PostalCode)) newUser.PostalCode = user.PostalCode;
                    if (user.Country != null && util.stringValid(user.Country)) newUser.Country = user.Country;
                    if (user.MartialStatus != null && util.stringValid(user.MartialStatus)) newUser.MartialStatus = user.MartialStatus;
                    if (user.ContactName != null && util.stringValid(user.ContactName)) newUser.ContactName = user.ContactName;
                    if (user.Relationship != null && util.stringValid(user.Relationship)) newUser.Relationship = user.Relationship;
                    if (user.PhoneR != null && util.stringValid(user.PhoneR)) newUser.PhoneR = user.PhoneR;
                    if (user.StreetR != null && util.stringValid(user.StreetR)) newUser.StreetR = user.StreetR;
                    if (user.FlatNumberR != null && util.stringValid(user.FlatNumberR)) newUser.FlatNumberR = user.FlatNumberR;
                    if (user.CityR != null && util.stringValid(user.CityR)) newUser.CityR = user.CityR;
                    if (user.ProvinceR != null && util.stringValid(user.ProvinceR)) newUser.ProvinceR = user.ProvinceR;
                    if (user.PostalCodeR != null && util.stringValid(user.PostalCodeR)) newUser.PostalCodeR = user.PostalCodeR;
                    if (user.CountryR != null && util.stringValid(user.CountryR)) newUser.CountryR = user.CountryR;

                    _db.Users.Add(newUser);

                    _db.SaveChanges();

                    var addUserRoleResult = urService.addUserRoles(newUser.Id.ToString(), user.Roles);
                    if (!addUserRoleResult.Success)
                    {
                        return new Result<Account>(false, addUserRoleResult.Message);
                    }

                    var addDepartmentMemberResult = dms.addDepartmentMembers(newUser.Id.ToString(), user.Departments);
                    if (!addDepartmentMemberResult.Success)
                    {
                        return new Result<Account>(false, addDepartmentMemberResult.Message);
                    }

                    return new Result<Account>(true, "Register new user successfully !", newUser);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Account>(false, "Internal error !");
            }
        }

        public Result<Account> editProfileService(string idUserEdit, [FromBody] AccountForAddDTO editUser)
        {
            try
            {
                var userId = Guid.Parse(idUserEdit);
                var user = _db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Id == userId);
                if (user == null)
                {
                    return new Result<Account>(false, "User does not exist !");
                }
                //Trace.WriteLine($"--AP: {user.AvatarPath}");
                if(editUser == null)
                {
                    return new Result<Account>(false, "Missing parameter(s) !");
                }

                //start edit fields
                if(editUser.Roles != null)
                {
                    var rs = urService.addUserRoles(idUserEdit, editUser.Roles);
                    if (!rs.Success)
                    {
                        return new Result<Account>(false, rs.Message);
                    }
                }
                if(editUser.Departments != null)
                {
                    var rs = dms.addDepartmentMembers(idUserEdit, editUser.Departments);
                    if (!rs.Success)
                    {
                        return new Result<Account>(false, rs.Message);
                    }
                }
                if (editUser.Birthday != null)
                {
                    try
                    {
                        user.Birthday = editUser.Birthday;
                        if (user.Birthday > DateTime.Now)
                        {
                            return new Result<Account>(false, "Birthday cannot exceed time in the future");
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Birthday");
                    }
                }
                if (editUser.StartingDateOfficial != null)
                {
                    try
                    {
                        user.StartingDateOfficial = editUser.StartingDateOfficial;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Starting Date Official");
                    }
                }
                if (editUser.LeavingDate != null)
                {
                    try
                    {
                        user.LeavingDate = editUser.LeavingDate;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Leaving Date");
                    }
                }
                if (editUser.StartDateMaternityLeave != null)
                {
                    try
                    {
                        user.StartDateMaternityLeave = editUser.StartDateMaternityLeave;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Start Date Maternity Leave");
                    }
                }

                if (editUser.Email != null && util.stringValid(editUser.Email) && !editUser.Email.Equals(user.Email))
                {
                    if(_db.Users.Where(u => u.Email == editUser.Email).Any())
                    {
                        return new Result<Account>(false, "Duplicate email, try changing to another one !");
                    }
                    if (!util.IsValidEmail(editUser.Email))
                    {
                        return new Result<Account>(false, "Wrong format of email !");
                    }
                    user.Email = editUser.Email;
                    user.Username = editUser.Email;
                }
                if (editUser.EmployeeNumber != null && util.stringValid(editUser.EmployeeNumber) && !editUser.EmployeeNumber.Equals(user.EmployeeNumber))
                {
                    if (_db.Users.FirstOrDefault(u => u.IsDeleted == false && u.EmployeeNumber == editUser.EmployeeNumber) != null)
                    {
                        return new Result<Account>(false, "Duplicate employee number !");
                    }
                    user.EmployeeNumber = editUser.EmployeeNumber;
                }
                if (editUser.AvatarPath != null && util.stringValid(editUser.AvatarPath))
                {
                    user.AvatarPath = editUser.AvatarPath;
                }
                if (editUser.FirstName != null && util.stringValid(editUser.FirstName)) user.FirstName = editUser.FirstName;
                if (editUser.LastName != null && util.stringValid(editUser.LastName)) user.LastName = editUser.LastName;
                if (editUser.Sex != null)
                {
                    try
                    {
                        user.Sex = editUser.Sex;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Type of sex is invalid !");
                    }
                }
                if (editUser.JobTitle != null && util.stringValid(editUser.JobTitle)) user.JobTitle = editUser.JobTitle;
                if (editUser.Company != null && util.stringValid(editUser.Company)) user.Company = editUser.Company;
                if (editUser.Unit != null && util.stringValid(editUser.Unit)) user.Unit = editUser.Unit;
                if (editUser.Function != null && util.stringValid(editUser.Function)) user.Function = editUser.Function;
                if (editUser.SectionsOrTeam != null && util.stringValid(editUser.SectionsOrTeam)) user.SectionsOrTeam = editUser.SectionsOrTeam;
                if (editUser.Groups != null && util.stringValid(editUser.Groups)) user.Groups = editUser.Groups;
                if (editUser.OfficeLocation != null && util.stringValid(editUser.OfficeLocation)) user.OfficeLocation = editUser.OfficeLocation;
                if (editUser.LineManager != null && util.stringValid(editUser.LineManager)) user.LineManager = editUser.LineManager;
                if (editUser.BelongToDepartments != null && util.stringValid(editUser.BelongToDepartments)) user.BelongToDepartments = editUser.BelongToDepartments;
                if (editUser.DateOfIdCard != null)
                {
                    try
                    {
                        user.DateOfIdCard = editUser.DateOfIdCard;
                        if (user.DateOfIdCard > DateTime.Now)
                        {
                            return new Result<Account>(false, "Date Of Id Card cannot exceed time in the future");
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Date Of Id Card");
                    }
                }

                if (editUser.StartingDate != null)
                {
                    try
                    {
                        user.StartingDate = editUser.StartingDate;
                        if (user.StartingDate > DateTime.Now)
                        {
                            return new Result<Account>(false, "Starting Date (Interviewed date) cannot exceed time in the future");
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Starting Date (Interviewed date)");
                    }
                }
                if (editUser.Rank != null && util.stringValid(editUser.Rank)) user.Rank = editUser.Rank;
                if (editUser.EmployeeType != null && util.stringValid(editUser.EmployeeType)) user.EmployeeType = editUser.EmployeeType;
                if (editUser.Rights != null && util.stringValid(editUser.Rights)) user.Rights = editUser.Rights;
                if (editUser.Nation != null && util.stringValid(editUser.Nation)) user.Nation = editUser.Nation;
                if (editUser.IdCardNumber != null && util.stringValid(editUser.IdCardNumber))
                {
                    if (!Regex.IsMatch(editUser.IdCardNumber, @"^\d+$"))
                    {
                        return new Result<Account>(false, "Id card number cannot contain characters which is not number !");
                    }
                    user.IdCardNumber = editUser.IdCardNumber;
                }
                if (editUser.Phone != null && util.stringValid(editUser.Phone))
                {
                    if (!Regex.IsMatch(editUser.Phone, @"^\d+$"))
                    {
                        return new Result<Account>(false, "Phone number cannot contain characters which is not number !");
                    }
                    user.Phone = editUser.Phone;
                }
                if (editUser.PlaceOfIdCard != null && util.stringValid(editUser.PlaceOfIdCard)) user.PlaceOfIdCard = editUser.PlaceOfIdCard;
                if (editUser.HealthInsurance != null && util.stringValid(editUser.HealthInsurance)) user.HealthInsurance = editUser.HealthInsurance;
                if (editUser.Note != null && util.stringValid(editUser.Note)) user.Note = editUser.Note;
                if (editUser.AcademicLevel != null && util.stringValid(editUser.AcademicLevel)) user.AcademicLevel = editUser.AcademicLevel;
                if (editUser.Qualification != null && util.stringValid(editUser.Qualification)) user.Qualification = editUser.Qualification;
                if (editUser.BusinessPhone != null && util.stringValid(editUser.BusinessPhone)) user.BusinessPhone = editUser.BusinessPhone;
                if (editUser.HomePhone != null && util.stringValid(editUser.HomePhone)) user.HomePhone = editUser.HomePhone;
                if (editUser.PersonalEmail != null && util.stringValid(editUser.PersonalEmail)) user.PersonalEmail = editUser.PersonalEmail;
                if (editUser.BankName != null && util.stringValid(editUser.BankName)) user.BankName = editUser.BankName;
                if (editUser.BankBranchNumber != null && util.stringValid(editUser.BankBranchNumber))
                {
                    if (!Regex.IsMatch(editUser.BankBranchNumber, @"^\d+$"))
                    {
                        return new Result<Account>(false, "Bank branch number cannot contain characters which is not number !");
                    }
                    user.BankBranchNumber = editUser.BankBranchNumber;
                }
                if (editUser.BankBranchName != null && util.stringValid(editUser.BankBranchName)) user.BankBranchName = editUser.BankBranchName;
                if (editUser.BankAccountNumber != null && util.stringValid(editUser.BankAccountNumber))
                {
                    if (!Regex.IsMatch(editUser.BankAccountNumber, @"^\d+$"))
                    {
                        return new Result<Account>(false, "Bank account number cannot contain characters which is not number !");
                    }
                    user.BankAccountNumber = editUser.BankAccountNumber;
                } 
                if (editUser.BankAccountName != null && util.stringValid(editUser.BankAccountName)) user.BankAccountName = editUser.BankAccountName;
                if (editUser.Street != null && util.stringValid(editUser.Street)) user.Street = editUser.Street;
                if (editUser.FlatNumber != null && util.stringValid(editUser.FlatNumber)) user.FlatNumber = editUser.FlatNumber;
                if (editUser.City != null && util.stringValid(editUser.City)) user.City = editUser.City;
                if (editUser.Province != null && util.stringValid(editUser.Province)) user.Province = editUser.Province;
                if (editUser.PostalCode != null && util.stringValid(editUser.PostalCode)) user.PostalCode = editUser.PostalCode;
                if (editUser.Country != null && util.stringValid(editUser.Country)) user.Country = editUser.Country;
                if (editUser.MartialStatus != null && util.stringValid(editUser.MartialStatus)) user.MartialStatus = editUser.MartialStatus;
                if (editUser.ContactName != null && util.stringValid(editUser.ContactName)) user.ContactName = editUser.ContactName;
                if (editUser.Relationship != null && util.stringValid(editUser.Relationship)) user.Relationship = editUser.Relationship;
                if (editUser.PhoneR != null && util.stringValid(editUser.PhoneR))
                {
                    if (!int.TryParse(editUser.PhoneR, out _))
                    {
                        return new Result<Account>(false, "Phone number of family member cannot contain characters which is not number !");
                    }
                    user.PhoneR = editUser.PhoneR;
                }
                if (editUser.StreetR != null && util.stringValid(editUser.StreetR)) user.StreetR = editUser.StreetR;
                if (editUser.FlatNumberR != null && util.stringValid(editUser.FlatNumberR)) user.FlatNumberR = editUser.FlatNumberR;
                if (editUser.CityR != null && util.stringValid(editUser.CityR)) user.CityR = editUser.CityR;
                if (editUser.ProvinceR != null && util.stringValid(editUser.ProvinceR)) user.ProvinceR = editUser.ProvinceR;
                if (editUser.PostalCodeR != null && util.stringValid(editUser.PostalCodeR)) user.PostalCodeR = editUser.PostalCodeR;
                if (editUser.CountryR != null && util.stringValid(editUser.CountryR)) user.CountryR = editUser.CountryR;
                //user.Created = DateTime.Now;

                _db.SaveChanges();
                return new Result<Account>(true, "Edit profile successfully !", user);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return new Result<Account>(false, "Internal error !");
            }
        }

        public Result<string> setSignature(Account user)
        {
            try
            {
                if(user == null && user.Signature == null && user.Id == null)
                {
                    return new Result<string>(false, "Missing parameter(s) or invalid id user !");
                }
                if(!util.stringValid(user.Signature))
                {
                    return new Result<string>(false, "Signature cannot be empty !");
                }
                var userId = user.Id;
                var signature = user.Signature;
                var exist = _db.Users.FirstOrDefault(u => u.Id == userId && u.IsDeleted == false);
                if(exist == null)
                {
                    return new Result<string>(false, "User does not exist !");
                }
                exist.Signature = signature;
                _db.SaveChanges();
                return new Result<string>(true, "Set signature successfully !");
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "");
            }
        }

        public Result<string> deleteUser(string id)
        {
            try
            {
                if(id == null)
                {
                    return new Result<string>(false, "Missing id of user !");
                }
                var userId = Guid.Parse(id);
                var user = _db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Id == userId);
                var userRole = _db.UserRoles.FirstOrDefault(u => u.IsDeleted == false && u.UserId == userId);
                var departmentMember = _db.DepartmentsMembers.FirstOrDefault(u => u.IsDeleted == false && u.UserId == userId);
                if (user == null)
                {
                    return new Result<string>(false, "User with input id does not exist !");
                }
                if(userRole != null || departmentMember != null)
                {
                    return new Result<string>(false, "Cannot delete, this user has some related data in other places !");
                }
                _db.Users.Remove(user);
                _db.SaveChanges();
                return new Result<string>(true, "Delete user successfully !");
            }
            catch(DbUpdateException e)
            {
                Trace.WriteLine(e.InnerException);
                return new Result<string>(false, "Internal error !");
            }
        }
    }
}