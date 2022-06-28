using FluentMigrator;

namespace WL.Migration.Migrations._2022;

[Migration(202206031314, "WISH-1. Create init tables for Wishlist App")]
public class V1_280620220011_initDb : FluentMigrator.Migration {
    public override void Up() {
        Create.Table("user")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("nickname").AsString(255).NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Table("wish")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("description").AsString(255).NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Table("user_wishes")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("user_id").AsInt64().ForeignKey("user", "id").NotNullable()
            .WithColumn("wish_id").AsInt64().ForeignKey("wish", "id").Nullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Table("basket")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("user_id").AsInt64().ForeignKey("user", "id").NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Table("basket_wishes")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("basket_id").AsInt64().ForeignKey("basket", "id").NotNullable()
            .WithColumn("wish_id").AsInt64().ForeignKey("wish", "id").Nullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
    }

    public override void Down() {
        Delete.Table("user");
        Delete.Table("wish");
        Delete.Table("user_wishes");
        Delete.Table("basket");
        Delete.Table("basket_wishes");
    }
}