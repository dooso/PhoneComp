namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usedworkerRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UsedWorker", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.UsedWorker", "IDcard", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UsedWorker", "IDcard", c => c.String());
            AlterColumn("dbo.UsedWorker", "Name", c => c.String());
        }
    }
}
