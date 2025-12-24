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
            migrationBuilder.CreateIndex(
                name: "IX_Translations_Culture_Key",
                table: "Translations",
                columns: new[] { "Culture", "Key" },
                unique: true,
                filter: "[TenantId] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Translations_Culture_Key",
                table: "Translations");
        }
    }
}
