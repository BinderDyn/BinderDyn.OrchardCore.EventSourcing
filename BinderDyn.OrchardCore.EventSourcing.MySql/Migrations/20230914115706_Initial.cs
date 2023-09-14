using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinderDyn.OrchardCore.EventSourcing.MySql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false),
                    OriginalEventEventId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ReferenceId = table.Column<string>(type: "longtext", nullable: true),
                    Payload = table.Column<string>(type: "longtext", nullable: false),
                    PayloadType = table.Column<string>(type: "longtext", nullable: false),
                    EventTypeFriendlyName = table.Column<string>(type: "longtext", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EventState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Events_OriginalEventEventId",
                        column: x => x.OriginalEventEventId,
                        principalTable: "Events",
                        principalColumn: "EventId");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Events_OriginalEventEventId",
                table: "Events",
                column: "OriginalEventEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
