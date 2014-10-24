namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dependationMemberRole : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.Member", "RoleID", "dbo.Role", "RoleID", cascadeDelete: true);
            CreateIndex("dbo.Member", "RoleID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Member", new[] { "RoleID" });
            DropForeignKey("dbo.Member", "RoleID", "dbo.Role");
        }
    }
}
