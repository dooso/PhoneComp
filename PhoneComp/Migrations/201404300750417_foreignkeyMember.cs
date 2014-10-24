namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignkeyMember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Member", "CreateMemberID", c => c.Int());
            AddForeignKey("dbo.UsedWorker", "CreateUserID", "dbo.Member", "MemberID");
            AddForeignKey("dbo.Suspects", "CreateUserID", "dbo.Member", "MemberID");
            AddForeignKey("dbo.CallRecord", "CreateUserID", "dbo.Member", "MemberID");
            AddForeignKey("dbo.Contact", "CreateUserID", "dbo.Member", "MemberID");
            CreateIndex("dbo.UsedWorker", "CreateUserID");
            CreateIndex("dbo.Suspects", "CreateUserID");
            CreateIndex("dbo.CallRecord", "CreateUserID");
            CreateIndex("dbo.Contact", "CreateUserID");
            DropColumn("dbo.Member", "CreateUserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Member", "CreateUserID", c => c.Int());
            DropIndex("dbo.Contact", new[] { "CreateUserID" });
            DropIndex("dbo.CallRecord", new[] { "CreateUserID" });
            DropIndex("dbo.Suspects", new[] { "CreateUserID" });
            DropIndex("dbo.UsedWorker", new[] { "CreateUserID" });
            DropForeignKey("dbo.Contact", "CreateUserID", "dbo.Member");
            DropForeignKey("dbo.CallRecord", "CreateUserID", "dbo.Member");
            DropForeignKey("dbo.Suspects", "CreateUserID", "dbo.Member");
            DropForeignKey("dbo.UsedWorker", "CreateUserID", "dbo.Member");
            DropColumn("dbo.Member", "CreateMemberID");
        }
    }
}
