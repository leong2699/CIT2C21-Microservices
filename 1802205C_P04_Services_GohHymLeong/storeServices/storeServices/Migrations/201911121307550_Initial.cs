namespace storeServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        StoreID = c.Int(nullable: false, identity: true),
                        MallID = c.Int(nullable: false),
                        StoreName = c.String(),
                        CoverImage = c.Binary(),
                        OperatingHours = c.String(),
                        ContactNumber = c.String(),
                        StoreDescription = c.String(),
                        Level = c.String(),
                    })
                .PrimaryKey(t => t.StoreID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Stores");
        }
    }
}
