namespace orderServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TotalPrice", c => c.String());
            AddColumn("dbo.Orders", "Payer", c => c.String());
            DropColumn("dbo.Orders", "ProductID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "ProductID", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "Payer");
            DropColumn("dbo.Orders", "TotalPrice");
        }
    }
}
