namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addrole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        IsDeleted = c.Boolean(nullable: false),
                        CreateUserID = c.Int(),
                        LastCreateUserID = c.Int(),
                        CreateDate = c.DateTime(),
                        LastUpdateDate = c.DateTime(),
                        RoleName = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RoleID);
            
            AddColumn("dbo.Member", "UserName", c => c.String());
            AddColumn("dbo.Member", "RealName", c => c.String());
            AddColumn("dbo.Member", "Password", c => c.String());
            AddColumn("dbo.Member", "PasswordNotMD5", c => c.String());
            AddColumn("dbo.Member", "RoleID", c => c.Int(nullable: false));
            AddColumn("dbo.Member", "Job", c => c.String());
            AddColumn("dbo.Member", "Address", c => c.String());
            AddColumn("dbo.Member", "Remark", c => c.String());
            DropColumn("dbo.Member", "Name");
            DropColumn("dbo.Member", "Role");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Member", "Role", c => c.Int(nullable: false));
            AddColumn("dbo.Member", "Name", c => c.String());
            DropColumn("dbo.Member", "Remark");
            DropColumn("dbo.Member", "Address");
            DropColumn("dbo.Member", "Job");
            DropColumn("dbo.Member", "RoleID");
            DropColumn("dbo.Member", "PasswordNotMD5");
            DropColumn("dbo.Member", "Password");
            DropColumn("dbo.Member", "RealName");
            DropColumn("dbo.Member", "UserName");
            DropTable("dbo.Role");
        }
    }
}
