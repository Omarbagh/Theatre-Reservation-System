using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarterKit.Migrations
{
    /// <inheritdoc />
    public partial class admindashbordupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminDashboards",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TheatreShowId = table.Column<int>(type: "INTEGER", nullable: false),
                    VenueId = table.Column<int>(type: "INTEGER", nullable: false),
                    AmountOfTickets = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    SnacksDetails = table.Column<string>(type: "TEXT", nullable: true),
                    DateAndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReservationUsed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminDashboards", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_AdminDashboards_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminDashboards_TheatreShow_TheatreShowId",
                        column: x => x.TheatreShowId,
                        principalTable: "TheatreShow",
                        principalColumn: "TheatreShowId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminDashboards_Venue_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venue",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminDashboards_CustomerId",
                table: "AdminDashboards",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminDashboards_TheatreShowId",
                table: "AdminDashboards",
                column: "TheatreShowId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminDashboards_VenueId",
                table: "AdminDashboards",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminDashboards");
        }
    }
}
