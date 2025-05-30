using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationPhoneToPurchaseOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotificationPhone",
                table: "PurchaseOrders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationPhone",
                table: "PurchaseOrders");
        }
    }
}
