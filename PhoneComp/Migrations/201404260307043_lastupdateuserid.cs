namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lastupdateuserid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UsedWorker", "LastUpdateUserID", c => c.Int());
            AddColumn("dbo.Suspects", "LastUpdateUserID", c => c.Int());
            AddColumn("dbo.CallRecord", "LastUpdateUserID", c => c.Int());
            AddColumn("dbo.Contact", "LastUpdateUserID", c => c.Int());
            AddColumn("dbo.Member", "LastUpdateUserID", c => c.Int());
            AddColumn("dbo.Member", "LastLoginTime", c => c.DateTime());
            AddColumn("dbo.Role", "LastUpdateUserID", c => c.Int());
            DropColumn("dbo.UsedWorker", "LastCreateUserID");
            DropColumn("dbo.Suspects", "LastCreateUserID");
            DropColumn("dbo.CallRecord", "LastCreateUserID");
            DropColumn("dbo.Contact", "LastCreateUserID");
            DropColumn("dbo.Member", "LastCreateUserID");
            DropColumn("dbo.Role", "LastCreateUserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Role", "LastCreateUserID", c => c.Int());
            AddColumn("dbo.Member", "LastCreateUserID", c => c.Int());
            AddColumn("dbo.Contact", "LastCreateUserID", c => c.Int());
            AddColumn("dbo.CallRecord", "LastCreateUserID", c => c.Int());
            AddColumn("dbo.Suspects", "LastCreateUserID", c => c.Int());
            AddColumn("dbo.UsedWorker", "LastCreateUserID", c => c.Int());
            DropColumn("dbo.Role", "LastUpdateUserID");
            DropColumn("dbo.Member", "LastLoginTime");
            DropColumn("dbo.Member", "LastUpdateUserID");
            DropColumn("dbo.Contact", "LastUpdateUserID");
            DropColumn("dbo.CallRecord", "LastUpdateUserID");
            DropColumn("dbo.Suspects", "LastUpdateUserID");
            DropColumn("dbo.UsedWorker", "LastUpdateUserID");
        }
    }
}
