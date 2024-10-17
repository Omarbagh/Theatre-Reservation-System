using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarterKit.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerIdAndTheatreShowDateIdToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Customer_CustomerId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_TheatreShowDate_TheatreShowDateId",
                table: "Reservation");

            migrationBuilder.AlterColumn<int>(
                name: "TheatreShowDateId",
                table: "Reservation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Reservation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Customer_CustomerId",
                table: "Reservation",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_TheatreShowDate_TheatreShowDateId",
                table: "Reservation",
                column: "TheatreShowDateId",
                principalTable: "TheatreShowDate",
                principalColumn: "TheatreShowDateId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Customer_CustomerId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_TheatreShowDate_TheatreShowDateId",
                table: "Reservation");

            migrationBuilder.AlterColumn<int>(
                name: "TheatreShowDateId",
                table: "Reservation",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Reservation",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Customer_CustomerId",
                table: "Reservation",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_TheatreShowDate_TheatreShowDateId",
                table: "Reservation",
                column: "TheatreShowDateId",
                principalTable: "TheatreShowDate",
                principalColumn: "TheatreShowDateId");
        }
    }
}
