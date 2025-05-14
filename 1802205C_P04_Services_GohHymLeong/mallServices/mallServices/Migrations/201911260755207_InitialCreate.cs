namespace mallServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Malls",
                c => new
                    {
                        MallID = c.Int(nullable: false, identity: true),
                        MallName = c.String(nullable: false),
                        MallImage = c.Binary(),
                        Description = c.String(),
                        CarparkCode = c.String(),
                        Location = c.String(),
                        Latitude = c.Int(nullable: false),
                        Longitude = c.Int(nullable: false),
                        OpeningHours = c.String(),
                        OpeningDate = c.String(),
                    })
                .PrimaryKey(t => t.MallID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Malls");
        }
    }
}
