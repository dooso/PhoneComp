namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaxLengthAndRequried : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suspects", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Suspects", "SuspectName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Suspects", "SuspectMobile", c => c.String(nullable: false, maxLength: 24));
            AlterColumn("dbo.CallRecord", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Contact", "LinkerName", c => c.String(nullable: false));
            AlterColumn("dbo.Contact", "LinkerMobile", c => c.String(nullable: false));
            AlterColumn("dbo.Contact", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contact", "IsDeleted", c => c.Boolean());
            AlterColumn("dbo.Contact", "LinkerMobile", c => c.String());
            AlterColumn("dbo.Contact", "LinkerName", c => c.String());
            AlterColumn("dbo.CallRecord", "IsDeleted", c => c.Boolean());
            AlterColumn("dbo.Suspects", "SuspectMobile", c => c.String());
            AlterColumn("dbo.Suspects", "SuspectName", c => c.String());
            DropColumn("dbo.Suspects", "IsDeleted");
        }
    }
}
