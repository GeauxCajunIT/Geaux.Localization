using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geaux.Localization.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalizationActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationCultures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CultureCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FlagCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsRightToLeft = table.Column<bool>(type: "bit", nullable: false),
                    RequiresReview = table.Column<bool>(type: "bit", nullable: false),
                    PercentTranslated = table.Column<double>(type: "float", nullable: false),
                    FallbackCulture = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationCultures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalizationKeyId = table.Column<int>(type: "int", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalizationValues_LocalizationKeys_LocalizationKeyId",
                        column: x => x.LocalizationKeyId,
                        principalTable: "LocalizationKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationCultures_CultureCode",
                table: "LocalizationCultures",
                column: "CultureCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationKeys_Key",
                table: "LocalizationKeys",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationValues_LocalizationKeyId_Culture",
                table: "LocalizationValues",
                columns: new[] { "LocalizationKeyId", "Culture" },
                unique: true,
                filter: "[TenantId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationValues_TenantId_LocalizationKeyId_Culture",
                table: "LocalizationValues",
                columns: new[] { "TenantId", "LocalizationKeyId", "Culture" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalizationActivityLogs");

            migrationBuilder.DropTable(
                name: "LocalizationCultures");

            migrationBuilder.DropTable(
                name: "LocalizationValues");

            migrationBuilder.DropTable(
                name: "LocalizationKeys");
        }
    }
}
