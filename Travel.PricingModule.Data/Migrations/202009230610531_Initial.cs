namespace Travel.PricingModule.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
             CreateTable(
                "dbo.PricelistVendors",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Vendors = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pricelist", t => t.Id)
                .Index(t => t.Id);

					Sql("INSERT INTO [dbo].[PricelistVendors](Id)(SELECT Id FROM[dbo].[Pricelist] WHERE Id NOT IN(SELECT Id FROM[dbo].[PricelistVendors]))");

				}
        
        public override void Down()
        {
						Sql("DELETE FROM [dbo].[PricelistVendors] WHERE Id in (SELECT Id FROM[dbo].[Pricelist])");
						DropForeignKey("dbo.PricelistVendors", "Id", "dbo.Pricelist");
           
            DropIndex("dbo.PricelistVendors", new[] { "Id" });
           
            DropTable("dbo.PricelistVendors");
           
        }
    }
}
