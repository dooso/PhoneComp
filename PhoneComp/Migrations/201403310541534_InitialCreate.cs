namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UsedWorker",
                c => new
                    {
                        UsedWorkerID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IDcard = c.String(),
                        Job = c.String(),
                        JobAddress = c.String(),
                        HomeAddress = c.String(),
                        Phone1 = c.String(),
                        Phone2 = c.String(),
                        Phone3 = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UsedWorkerID);
            
            CreateTable(
                "dbo.Suspects",
                c => new
                    {
                        SuspectID = c.Int(nullable: false, identity: true),
                        SuspectName = c.String(),
                        SuspectMobile = c.String(),
                    })
                .PrimaryKey(t => t.SuspectID);
            
            CreateTable(
                "dbo.CallRecord",
                c => new
                    {
                        CallRecordID = c.Int(nullable: false, identity: true),
                        SuspectID = c.Int(nullable: false),
                        CalledMobile = c.String(),
                        LordCalled = c.String(),
                        CallTime = c.DateTime(),
                        CallDuration = c.String(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.CallRecordID)
                .ForeignKey("dbo.Suspects", t => t.SuspectID, cascadeDelete: true)
                .Index(t => t.SuspectID);
            
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        ContactID = c.Int(nullable: false, identity: true),
                        SuspectID = c.Int(nullable: false),
                        LinkerName = c.String(),
                        LinkerMobile = c.String(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ContactID)
                .ForeignKey("dbo.Suspects", t => t.SuspectID, cascadeDelete: true)
                .Index(t => t.SuspectID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Contact", new[] { "SuspectID" });
            DropIndex("dbo.CallRecord", new[] { "SuspectID" });
            DropForeignKey("dbo.Contact", "SuspectID", "dbo.Suspects");
            DropForeignKey("dbo.CallRecord", "SuspectID", "dbo.Suspects");
            DropTable("dbo.Contact");
            DropTable("dbo.CallRecord");
            DropTable("dbo.Suspects");
            DropTable("dbo.UsedWorker");
        }
    }
}
