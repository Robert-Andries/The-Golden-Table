using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoldenTable.Modules.Catalog.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.CreateTable(
                name: "Dishes",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description_Value = table.Column<string>(type: "text", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    BasePrice_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    BasePrice_Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    NutritionalInformation = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishTag",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishSize",
                schema: "Catalog",
                columns: table => new
                {
                    DishId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PriceAdded = table.Column<float>(type: "real", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishSize", x => new { x.DishId, x.Id });
                    table.ForeignKey(
                        name: "FK_DishSize_Dishes_DishId",
                        column: x => x.DishId,
                        principalSchema: "Catalog",
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description_Value = table.Column<string>(type: "text", nullable: true),
                    DishId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Dishes_DishId",
                        column: x => x.DishId,
                        principalSchema: "Catalog",
                        principalTable: "Dishes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DishDishTag",
                schema: "Catalog",
                columns: table => new
                {
                    DishId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishDishTag", x => new { x.DishId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_DishDishTag_DishTag_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "Catalog",
                        principalTable: "DishTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishDishTag_Dishes_DishId",
                        column: x => x.DishId,
                        principalSchema: "Catalog",
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishDishTag_TagsId",
                schema: "Catalog",
                table: "DishDishTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_Name",
                schema: "Catalog",
                table: "Dishes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_DishId",
                schema: "Catalog",
                table: "Images",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Name",
                schema: "Catalog",
                table: "Images",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishDishTag",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "DishSize",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Images",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "DishTag",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Dishes",
                schema: "Catalog");
        }
    }
}
