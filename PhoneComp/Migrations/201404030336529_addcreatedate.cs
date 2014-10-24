namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcreatedate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UsedWorker", "CreateDate", c => c.DateTime());
            AddColumn("dbo.UsedWorker", "LastUpdateDate", c => c.DateTime());
            AddColumn("dbo.Suspects", "CreateDate", c => c.DateTime());
            AddColumn("dbo.Suspects", "LastUpdateDate", c => c.DateTime());
            AddColumn("dbo.CallRecord", "CreateDate", c => c.DateTime());
            AddColumn("dbo.CallRecord", "LastUpdateDate", c => c.DateTime());
            AddColumn("dbo.Contact", "CreateDate", c => c.DateTime());
            AddColumn("dbo.Contact", "LastUpdateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contact", "LastUpdateDate");
            DropColumn("dbo.Contact", "CreateDate");
            DropColumn("dbo.CallRecord", "LastUpdateDate");
            DropColumn("dbo.CallRecord", "CreateDate");
            DropColumn("dbo.Suspects", "LastUpdateDate");
            DropColumn("dbo.Suspects", "CreateDate");
            DropColumn("dbo.UsedWorker", "LastUpdateDate");
            DropColumn("dbo.UsedWorker", "CreateDate");
        }
    }
}
