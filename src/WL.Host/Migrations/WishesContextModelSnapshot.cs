// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WL.Host.DbContexts;

#nullable disable

namespace WL.Host.Migrations
{
    [DbContext(typeof(WishesContext))]
    partial class WishesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("WL.Host.Entities.Wish", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("WishCategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Wishes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7a964531-e3ba-4707-bcfe-70bb2617a4b5"),
                            Description = "Description of first willing",
                            Name = "первое желание",
                            WishCategoryId = 1
                        },
                        new
                        {
                            Id = new Guid("3e0c2112-5740-4008-b3c3-9fd6436116a7"),
                            Description = "Description of second willing",
                            Name = "второе желание",
                            WishCategoryId = 2
                        },
                        new
                        {
                            Id = new Guid("f2d8e8ed-42f7-4384-bdd7-9ddd63e0b40c"),
                            Description = "Description of third willing",
                            Name = "третье желание",
                            WishCategoryId = 2
                        });
                });

            modelBuilder.Entity("WL.Host.Entities.WishCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("WithCategories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Best"
                        },
                        new
                        {
                            Id = 2,
                            Name = "So so"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
