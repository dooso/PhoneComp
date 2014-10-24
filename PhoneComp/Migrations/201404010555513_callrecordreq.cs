namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class callrecordreq : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CallRecord", "CalledMobile", c => c.String(nullable: false));
            AlterColumn("dbo.CallRecord", "LordCalled", c => c.String(nullable: false));
            AlterColumn("dbo.CallRecord", "CallTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CallRecord", "CallTime", c => c.DateTime());
            AlterColumn("dbo.CallRecord", "LordCalled", c => c.String());
            AlterColumn("dbo.CallRecord", "CalledMobile", c => c.String());
        }
    }
}
