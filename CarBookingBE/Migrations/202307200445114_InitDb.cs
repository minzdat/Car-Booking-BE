namespace CarBookingBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Department",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        ContactInfo = c.String(),
                        Code = c.String(),
                        UnderDepartment = c.Guid(),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DepartmentMember",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Position = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        UserId = c.Guid(),
                        DepartmentId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Department", t => t.DepartmentId)
                .ForeignKey("dbo.Account", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Username = c.String(maxLength: 50),
                        Password = c.String(),
                        Email = c.String(maxLength: 50),
                        EmployeeNumber = c.String(maxLength: 30),
                        AvatarPath = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Sex = c.Boolean(),
                        Created = c.DateTime(),
                        Birthday = c.DateTime(),
                        JobTitle = c.String(),
                        Company = c.String(),
                        Unit = c.String(),
                        Function = c.String(),
                        SectionsOrTeam = c.String(),
                        Groups = c.String(),
                        OfficeLocation = c.String(),
                        LineManager = c.String(),
                        BelongToDepartments = c.String(),
                        Rank = c.String(),
                        EmployeeType = c.String(),
                        Rights = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Nation = c.String(),
                        Phone = c.String(),
                        IdCardNumber = c.String(),
                        DateOfIdCard = c.DateTime(),
                        PlaceOfIdCard = c.String(),
                        HealthInsurance = c.String(),
                        StartingDate = c.DateTime(),
                        StartingDateOfficial = c.DateTime(),
                        LeavingDate = c.DateTime(),
                        StartDateMaternityLeave = c.DateTime(),
                        Note = c.String(),
                        AcademicLevel = c.String(),
                        Qualification = c.String(),
                        BusinessPhone = c.String(),
                        HomePhone = c.String(),
                        PersonalEmail = c.String(),
                        BankName = c.String(),
                        BankBranchNumber = c.String(),
                        BankBranchName = c.String(),
                        BankAccountNumber = c.String(),
                        BankAccountName = c.String(),
                        Street = c.String(),
                        FlatNumber = c.String(),
                        City = c.String(),
                        Province = c.String(),
                        PostalCode = c.String(),
                        Country = c.String(),
                        MartialStatus = c.String(),
                        ContactName = c.String(),
                        Relationship = c.String(),
                        PhoneR = c.String(),
                        StreetR = c.String(),
                        FlatNumberR = c.String(),
                        CityR = c.String(),
                        ProvinceR = c.String(),
                        PostalCodeR = c.String(),
                        CountryR = c.String(),
                        Signature = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Username, unique: true)
                .Index(t => t.Email, unique: true)
                .Index(t => t.EmployeeNumber, unique: true);
            
            CreateTable(
                "dbo.Request",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RequestCode = c.String(),
                        SenderId = c.Guid(),
                        DepartmentId = c.Guid(),
                        ReceiverId = c.Guid(),
                        Created = c.DateTime(),
                        Mobile = c.String(),
                        CostCenter = c.String(),
                        TotalPassengers = c.Int(),
                        UsageFrom = c.DateTime(),
                        UsageTo = c.DateTime(),
                        PickTime = c.DateTime(),
                        PickLocation = c.String(),
                        Destination = c.String(),
                        Reason = c.String(),
                        Note = c.String(),
                        ApplyNote = c.Boolean(),
                        Status = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Account_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Department", t => t.DepartmentId)
                .ForeignKey("dbo.Account", t => t.ReceiverId)
                .ForeignKey("dbo.Account", t => t.SenderId)
                .ForeignKey("dbo.Account", t => t.Account_Id)
                .Index(t => t.SenderId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ReceiverId)
                .Index(t => t.Account_Id);
            
            CreateTable(
                "dbo.RequestAttachment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Path = c.String(),
                        RequestId = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .Index(t => t.RequestId);
            
            CreateTable(
                "dbo.RequestComment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(),
                        Content = c.String(),
                        Created = c.DateTime(nullable: false),
                        RequestId = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Account", t => t.UserId)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .Index(t => t.UserId)
                .Index(t => t.RequestId);
            
            CreateTable(
                "dbo.RequestCommentAttachment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Path = c.String(),
                        RequestCommentId = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RequestComment", t => t.RequestCommentId)
                .Index(t => t.RequestCommentId);
            
            CreateTable(
                "dbo.RequestShare",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UserId = c.Guid(),
                        RequestId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .ForeignKey("dbo.Account", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RequestId);
            
            CreateTable(
                "dbo.RequestWorkflow",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(),
                        Level = c.Int(nullable: false),
                        Status = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        RequestId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .ForeignKey("dbo.Account", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RequestId);
            
            CreateTable(
                "dbo.VehicleRequest",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RequestId = c.Guid(),
                        DriverId = c.Guid(),
                        DriverMobile = c.String(),
                        DriverCarplate = c.String(),
                        RotaionId = c.Int(),
                        Reason = c.String(),
                        Note = c.String(),
                        Type = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Request", t => t.RequestId)
                .ForeignKey("dbo.Rotation", t => t.RotaionId)
                .ForeignKey("dbo.Account", t => t.DriverId)
                .Index(t => t.RequestId)
                .Index(t => t.DriverId)
                .Index(t => t.RotaionId);
            
            CreateTable(
                "dbo.Rotation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AccountRole",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UserId = c.Guid(),
                        RoleId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoleOfAccount", t => t.RoleId)
                .ForeignKey("dbo.Account", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.RoleOfAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountRole", "UserId", "dbo.Account");
            DropForeignKey("dbo.AccountRole", "RoleId", "dbo.RoleOfAccount");
            DropForeignKey("dbo.Request", "Account_Id", "dbo.Account");
            DropForeignKey("dbo.VehicleRequest", "DriverId", "dbo.Account");
            DropForeignKey("dbo.VehicleRequest", "RotaionId", "dbo.Rotation");
            DropForeignKey("dbo.VehicleRequest", "RequestId", "dbo.Request");
            DropForeignKey("dbo.Request", "SenderId", "dbo.Account");
            DropForeignKey("dbo.RequestWorkflow", "UserId", "dbo.Account");
            DropForeignKey("dbo.RequestWorkflow", "RequestId", "dbo.Request");
            DropForeignKey("dbo.RequestShare", "UserId", "dbo.Account");
            DropForeignKey("dbo.RequestShare", "RequestId", "dbo.Request");
            DropForeignKey("dbo.RequestCommentAttachment", "RequestCommentId", "dbo.RequestComment");
            DropForeignKey("dbo.RequestComment", "RequestId", "dbo.Request");
            DropForeignKey("dbo.RequestComment", "UserId", "dbo.Account");
            DropForeignKey("dbo.RequestAttachment", "RequestId", "dbo.Request");
            DropForeignKey("dbo.Request", "ReceiverId", "dbo.Account");
            DropForeignKey("dbo.Request", "DepartmentId", "dbo.Department");
            DropForeignKey("dbo.DepartmentMember", "UserId", "dbo.Account");
            DropForeignKey("dbo.DepartmentMember", "DepartmentId", "dbo.Department");
            DropIndex("dbo.AccountRole", new[] { "RoleId" });
            DropIndex("dbo.AccountRole", new[] { "UserId" });
            DropIndex("dbo.VehicleRequest", new[] { "RotaionId" });
            DropIndex("dbo.VehicleRequest", new[] { "DriverId" });
            DropIndex("dbo.VehicleRequest", new[] { "RequestId" });
            DropIndex("dbo.RequestWorkflow", new[] { "RequestId" });
            DropIndex("dbo.RequestWorkflow", new[] { "UserId" });
            DropIndex("dbo.RequestShare", new[] { "RequestId" });
            DropIndex("dbo.RequestShare", new[] { "UserId" });
            DropIndex("dbo.RequestCommentAttachment", new[] { "RequestCommentId" });
            DropIndex("dbo.RequestComment", new[] { "RequestId" });
            DropIndex("dbo.RequestComment", new[] { "UserId" });
            DropIndex("dbo.RequestAttachment", new[] { "RequestId" });
            DropIndex("dbo.Request", new[] { "Account_Id" });
            DropIndex("dbo.Request", new[] { "ReceiverId" });
            DropIndex("dbo.Request", new[] { "DepartmentId" });
            DropIndex("dbo.Request", new[] { "SenderId" });
            DropIndex("dbo.Account", new[] { "EmployeeNumber" });
            DropIndex("dbo.Account", new[] { "Email" });
            DropIndex("dbo.Account", new[] { "Username" });
            DropIndex("dbo.DepartmentMember", new[] { "DepartmentId" });
            DropIndex("dbo.DepartmentMember", new[] { "UserId" });
            DropTable("dbo.RoleOfAccount");
            DropTable("dbo.AccountRole");
            DropTable("dbo.Rotation");
            DropTable("dbo.VehicleRequest");
            DropTable("dbo.RequestWorkflow");
            DropTable("dbo.RequestShare");
            DropTable("dbo.RequestCommentAttachment");
            DropTable("dbo.RequestComment");
            DropTable("dbo.RequestAttachment");
            DropTable("dbo.Request");
            DropTable("dbo.Account");
            DropTable("dbo.DepartmentMember");
            DropTable("dbo.Department");
        }
    }
}
