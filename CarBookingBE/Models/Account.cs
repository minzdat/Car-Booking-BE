using Newtonsoft.Json;
using CarBookingBE.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarBookingBE.Models;

namespace CarBookingTest.Models
{
    [Table("Account")]
    public class Account
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Index(IsUnique = true)]
        [MaxLength(50)]
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(50)]
        public string Email { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(30)]
        public string EmployeeNumber { get; set; }
        public string AvatarPath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Sex { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Birthday { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string Unit { get; set; }
        public string Function { get; set; }
        public string SectionsOrTeam { get; set; }
        public string Groups { get; set; }
        public string OfficeLocation { get; set; }
        public string LineManager { get; set; }
        public string BelongToDepartments { get; set; }
        public string Rank { get; set; }
        public string EmployeeType { get; set; }
        public string Rights { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        /* Additional */
        public string Nation { get; set; }
        public string Phone { get; set; }
        public string IdCardNumber { get; set; }
        public DateTime? DateOfIdCard { get; set; }
        public string PlaceOfIdCard { get; set; }
        public string HealthInsurance { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? StartingDateOfficial { get; set; }
        public DateTime? LeavingDate { get; set; }
        public DateTime? StartDateMaternityLeave { get; set; }
        public string Note { get; set; }
        // Literacy
        public string AcademicLevel { get; set; }
        public string Qualification { get; set; }
        // ContactInfo
        public string BusinessPhone { get; set; }
        public string HomePhone { get; set; }
        public string PersonalEmail { get; set; }
        // BankAccount
        public string BankName { get; set; }
        public string BankBranchNumber { get; set; }
        public string BankBranchName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        // Address
        public string Street { get; set; }
        public string FlatNumber { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        /* Family */
        public string MartialStatus { get; set; }
        // Emergency Contact
        public string ContactName { get; set; }
        public string Relationship { get; set; }
        public string PhoneR { get; set; }
        // Permanent Address
        public string StreetR { get; set; }
        public string FlatNumberR { get; set; }
        public string CityR { get; set; }
        public string ProvinceR { get; set; }
        public string PostalCodeR { get; set; }
        public string CountryR { get; set; }

        public string Signature { get; set; }
        //[JsonIgnore]
        public virtual ICollection<AccountRole> UserRoles { get; set; }
        //[JsonIgnore]
        public virtual ICollection<DepartmentMember> DepartmentMembers { get; set; }
        [JsonIgnore]
        public virtual ICollection<Request> Requests { get; set; }
        [JsonIgnore]
        public virtual ICollection<RequestShare> RequestShares { get; set; }
        [JsonIgnore]
        public virtual ICollection<RequestWorkflow> RequestWorkflows { get; set; }
        [JsonIgnore]
        public virtual ICollection<VehicleRequest> VehicleRequests { get; set; }

        public static implicit operator Account(AccountDTO v)
        {
            throw new NotImplementedException();
        }
    }
}