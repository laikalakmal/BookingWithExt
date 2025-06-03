using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class availability_as_complextypr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price_Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Availability_Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Availability_RemainingSlots = table.Column<int>(type: "int", nullable: false),
                    Availability_IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Property = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomOptions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Inclusions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialOffers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CancellationPolicy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HolidayPackages_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Inclusions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exclusions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureDates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accommodation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Transportation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CancellationPolicy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPackages_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HolidayPackages");

            migrationBuilder.DropTable(
                name: "TourPackages");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
