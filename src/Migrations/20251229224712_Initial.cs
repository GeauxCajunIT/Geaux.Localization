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
                name: "LocalizationKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Culture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
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
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "LocalizationValues");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "LocalizationKeys");
        }
    }
}
