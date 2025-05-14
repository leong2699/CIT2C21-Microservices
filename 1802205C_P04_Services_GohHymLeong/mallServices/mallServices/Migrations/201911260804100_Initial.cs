namespace mallServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Malls", "Latitude", c => c.Double(nullable: false));
            AlterColumn("dbo.Malls", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Malls", "Longitude", c => c.Int(nullable: false));
            AlterColumn("dbo.Malls", "Latitude", c => c.Int(nullable: false));
        }
    }
}
