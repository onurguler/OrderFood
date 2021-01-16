using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderFood.Infrastructure.Migrations
{
    public partial class RenameOrderPaymentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnumPaymentStatus",
                table: "Orders",
                newName: "PaymentStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "Orders",
                newName: "EnumPaymentStatus");
        }
    }
}
