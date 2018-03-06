namespace NPQIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class frontpagecorrected : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.FrontPageReviews");
            DropIndex("dbo.FrontPageReviews", new[] { "Publication_PublicationID" });
            DropColumn("dbo.FrontPageReviews", "PublicationID");
            RenameColumn(table: "dbo.FrontPageReviews", name: "Publication_PublicationID", newName: "PublicationID");
            AddColumn("dbo.FrontPageReviews", "FrontPageReviewID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.FrontPageReviews", "PublicationID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.FrontPageReviews", "FrontPageReviewID");
            CreateIndex("dbo.FrontPageReviews", "PublicationID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.FrontPageReviews", new[] { "PublicationID" });
            DropPrimaryKey("dbo.FrontPageReviews");
            AlterColumn("dbo.FrontPageReviews", "PublicationID", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.FrontPageReviews", "FrontPageReviewID");
            RenameColumn(table: "dbo.FrontPageReviews", name: "PublicationID", newName: "Publication_PublicationID");
            AddColumn("dbo.FrontPageReviews", "PublicationID", c => c.Int(nullable: false, identity: true));
            CreateIndex("dbo.FrontPageReviews", "Publication_PublicationID");
            AddPrimaryKey("dbo.FrontPageReviews", "PublicationID");

        }
    }
}
