namespace PhoneComp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Member",
                c => new
                    {
                        MemberID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Mobile = c.String(),
                        IDcard = c.String(),
                        Role = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreateUserID = c.Int(),
                        LastCreateUserID = c.Int(),
                        CreateDate = c.DateTime(),
                        LastUpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MemberID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Member");
        }
    }
}
