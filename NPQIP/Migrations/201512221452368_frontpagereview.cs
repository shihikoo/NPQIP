namespace NPQIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class frontpagereview : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FrontPageReviews",
                c => new
                    {
                        PublicationID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        ChecklistID = c.Int(nullable: false),
                        OptionID = c.Int(),
                        Comments = c.String(),
                        Status = c.String(),
                        LastUpdateTime = c.DateTime(nullable: false),
                        Publication_PublicationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PublicationID)
                .ForeignKey("dbo.Options", t => t.OptionID)
                .ForeignKey("dbo.Publications", t => t.Publication_PublicationID)
                .ForeignKey("dbo.UserProfile", t => t.UserID)
                .ForeignKey("dbo.Checklists", t => t.ChecklistID)
                .Index(t => t.UserID)
                .Index(t => t.ChecklistID)
                .Index(t => t.OptionID)
                .Index(t => t.Publication_PublicationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FrontPageReviews", "ChecklistID", "dbo.Checklists");
            DropForeignKey("dbo.FrontPageReviews", "UserID", "dbo.UserProfile");
            DropForeignKey("dbo.FrontPageReviews", "Publication_PublicationID", "dbo.Publications");
            DropForeignKey("dbo.FrontPageReviews", "OptionID", "dbo.Options");
            DropIndex("dbo.FrontPageReviews", new[] { "Publication_PublicationID" });
            DropIndex("dbo.FrontPageReviews", new[] { "OptionID" });
            DropIndex("dbo.FrontPageReviews", new[] { "ChecklistID" });
            DropIndex("dbo.FrontPageReviews", new[] { "UserID" });
            DropTable("dbo.FrontPageReviews");
        }
    }
}
