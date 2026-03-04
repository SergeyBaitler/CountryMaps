using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CountryMaps.TerminalsLoader.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "offices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    city_code = table.Column<int>(type: "integer", nullable: false),
                    uuid = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    country_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    addr_country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    addr_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    addr_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    addr_house = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    work_time = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "office_phones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    comment = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    office_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_office_phones", x => x.id);
                    table.ForeignKey(
                        name: "FK_office_phones_offices_office_id",
                        column: x => x.office_id,
                        principalTable: "offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_office_phones_office_id",
                table: "office_phones",
                column: "office_id");

            migrationBuilder.CreateIndex(
                name: "IX_offices_code",
                table: "offices",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_offices_country_code_city_code",
                table: "offices",
                columns: new[] { "country_code", "city_code" });

            migrationBuilder.CreateIndex(
                name: "IX_offices_uuid",
                table: "offices",
                column: "uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "office_phones");

            migrationBuilder.DropTable(
                name: "offices");
        }
    }
}
