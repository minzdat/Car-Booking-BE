namespace CarBookingBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBv2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Department", "Code", c => c.String(maxLength: 20));
            AlterColumn("dbo.Request", "RequestCode", c => c.String(maxLength: 50));
            CreateIndex("dbo.Department", "Code", unique: true);
            CreateIndex("dbo.Request", "RequestCode", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Request", new[] { "RequestCode" });
            DropIndex("dbo.Department", new[] { "Code" });
            AlterColumn("dbo.Request", "RequestCode", c => c.String());
            AlterColumn("dbo.Department", "Code", c => c.String());
        }
    }
}
