using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarterKit.Migrations
{
    /// <inheritdoc />
    public partial class ReservationSnackNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReservationSnacks",
                columns: table => new
                {
                    ReservationSnackId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReservationId = table.Column<int>(type: "INTEGER", nullable: false),
                    SnacksId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPrice = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationSnacks", x => x.ReservationSnackId);
                    table.ForeignKey(
                        name: "FK_ReservationSnacks_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationSnacks_Snacks_SnacksId",
                        column: x => x.SnacksId,
                        principalTable: "Snacks",
                        principalColumn: "SnacksId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationSnacks_ReservationId",
                table: "ReservationSnacks",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationSnacks_SnacksId",
                table: "ReservationSnacks",
                column: "SnacksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationSnacks");
        }
    }
}
