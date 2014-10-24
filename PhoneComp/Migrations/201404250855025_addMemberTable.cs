namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMemberTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UsedWorker", "CreateUserID", c => c.Int());
            AddColumn("dbo.UsedWorker", "LastCreateUserID", c => c.Int());
            AddColumn("dbo.Suspects", "CreateUserID", c => c.Int());
            AddColumn("dbo.Suspects", "LastCreateUserID", c => c.Int());
            AddColumn("dbo.CallRecord", "CreateUserID", c => c.Int());
            AddColumn("dbo.CallRecord", "LastCreateUserID", c => c.Int());
            AddColumn("dbo.Contact", "CreateUserID", c => c.Int());
            AddColumn("dbo.Contact", "LastCreateUserID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contact", "LastCreateUserID");
            DropColumn("dbo.Contact", "CreateUserID");
            DropColumn("dbo.CallRecord", "LastCreateUserID");
            DropColumn("dbo.CallRecord", "CreateUserID");
            DropColumn("dbo.Suspects", "LastCreateUserID");
            DropColumn("dbo.Suspects", "CreateUserID");
            DropColumn("dbo.UsedWorker", "LastCreateUserID");
            DropColumn("dbo.UsedWorker", "CreateUserID");
        }
    }
}
