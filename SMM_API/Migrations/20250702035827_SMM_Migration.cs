using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMM_API.Migrations
{
    /// <inheritdoc />
    public partial class SMM_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProductPrice = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductImgAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ProductStock = table.Column<int>(type: "int", nullable: false),
                    ProductFeatures = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSpecifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductIngredients = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductUsageInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductWarrantyInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductAdditionalInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsRemoved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__B40CC6EDDA6E6476", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_Category",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "CategoryID");
                });

            // Add Order Status column
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            // Add IsCheckedOut to Cart
            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedOut",
                table: "Cart",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Modify ProductLog table
            migrationBuilder.AlterColumn<string>(
                name: "LogType",
                table: "ProductLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "QuantityChange",
                table: "ProductLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "ProductLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove added columns
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsCheckedOut",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "QuantityChange",
                table: "ProductLog");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "ProductLog");

            // Restore ProductLog column
            migrationBuilder.AlterColumn<string>(
                name: "LogType",
                table: "ProductLog",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User",
                table: "Role");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "ProductLog");

            migrationBuilder.DropTable(
                name: "UserLog");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}