namespace NPQIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class look : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserProfile", "UserName", c => c.String(nullable: false));
            AlterColumn("dbo.UserProfile", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserProfile", "Email", c => c.String());
            AlterColumn("dbo.UserProfile", "UserName", c => c.String());
        }
    }
}
