using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionsWebApi.Migrations
{
    /// <inheritdoc />
    public partial class RestrictionsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    APIKeyId = table.Column<int>(type: "int", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainRestrictions_APIKeys_APIKeyId",
                        column: x => x.APIKeyId,
                        principalTable: "APIKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IPRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    APIKeyId = table.Column<int>(type: "int", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPRestrictions_APIKeys_APIKeyId",
                        column: x => x.APIKeyId,
                        principalTable: "APIKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainRestrictions_APIKeyId",
                table: "DomainRestrictions",
                column: "APIKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_IPRestrictions_APIKeyId",
                table: "IPRestrictions",
                column: "APIKeyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainRestrictions");

            migrationBuilder.DropTable(
                name: "IPRestrictions");
        }
    }
}
