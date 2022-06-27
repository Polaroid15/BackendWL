using FluentMigrator;

namespace Backend.Migration.Migrations._2022; 

[Migration(202206031314, "WISH-1. Create init tables for Wishlist App")]
public class V1_280620220011_initDb : FluentMigrator.Migration {
    public override void Up() {
        // Create.Table("migr_test")
            // .WithColumn("id").AsInt32().Identity().PrimaryKey()
            // .WithColumn("name").AsString(255).NotNullable()
            // .WithColumn("created_at").AsDateTimeOffset().NotNullable()
            // .WithColumn("updated_at").AsDateTimeOffset().NotNullable();
    }

    public override void Down() {
        // Delete.Table("migr_test");
    }
}