using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IronMan.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    ActivationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Verified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordReset = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<int>(type: "int", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_DeleterId",
                        column: x => x.DeleterId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonRevoked = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefreshToken_Accounts_UserId",
                        column: x => x.UserId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Activated", "ActivationToken", "Created", "CreationTime", "CreatorId", "DeleterId", "DeletionTime", "Email", "FirstName", "IsDeleted", "LastModificationTime", "LastModifierUserId", "LastName", "PasswordHash", "PasswordReset", "ResetToken", "ResetTokenExpires", "Role", "Title", "Updated", "VerificationToken", "Verified" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 3, 8, 15, 36, 20, 844, DateTimeKind.Unspecified).AddTicks(7440), null, new DateTime(2024, 3, 8, 15, 36, 20, 844, DateTimeKind.Unspecified).AddTicks(9570), new DateTime(2024, 3, 8, 17, 36, 20, 838, DateTimeKind.Unspecified).AddTicks(6430), null, null, null, "Franklin@shearwatervf.com", "Franklin", false, null, null, "Ndlovu", "$2a$11$xccdiOHYF6NCiwv05bTS6OmBTebIXrq9YCHJpOTdUwnJ9D6rrdy7i", new DateTime(2024, 4, 17, 13, 38, 20, 888, DateTimeKind.Unspecified).AddTicks(5144), null, null, 0, "Mr", null, null, new DateTime(2024, 3, 8, 15, 38, 20, 275, DateTimeKind.Unspecified).AddTicks(850) },
                    { 2, new DateTime(2024, 3, 8, 15, 41, 5, 580, DateTimeKind.Unspecified).AddTicks(7190), null, new DateTime(2024, 3, 8, 15, 40, 30, 575, DateTimeKind.Unspecified).AddTicks(1440), new DateTime(2024, 3, 8, 17, 40, 30, 521, DateTimeKind.Unspecified).AddTicks(8190), null, null, null, "evstores@shearwatervf.com", "Batirai A. A.", false, null, null, "Rutsito", "$2a$11$x1fZNHZb56UDOT6G50O/Y.g8GcHo7lQ/voCe6DoBpAp55bLwjoD4S", null, null, null, 1, "Mr", null, null, new DateTime(2024, 3, 8, 15, 40, 46, 380, DateTimeKind.Unspecified).AddTicks(1770) },
                    { 3, new DateTime(2024, 3, 8, 15, 41, 5, 580, DateTimeKind.Unspecified).AddTicks(7190), null, new DateTime(2024, 3, 8, 15, 40, 30, 575, DateTimeKind.Unspecified).AddTicks(1440), new DateTime(2024, 3, 8, 17, 40, 30, 521, DateTimeKind.Unspecified).AddTicks(8190), null, null, null, "dought@shearwatervf.com", "Dought", false, null, null, "Tabona", "$2a$11$x1fZNHZb56UDOT6G50O/Y.g8GcHo7lQ/voCe6DoBpAp55bLwjoD4S", null, null, null, 2, "Mr", null, null, new DateTime(2024, 3, 8, 15, 40, 46, 380, DateTimeKind.Unspecified).AddTicks(1770) },
                    { 4, new DateTime(2024, 3, 8, 15, 41, 5, 580, DateTimeKind.Unspecified).AddTicks(7190), null, new DateTime(2024, 3, 8, 15, 40, 30, 575, DateTimeKind.Unspecified).AddTicks(1440), new DateTime(2024, 3, 8, 17, 40, 30, 521, DateTimeKind.Unspecified).AddTicks(8190), null, null, null, "lawrence@shearwatervf.com", "Lawrence", false, null, null, "Gundo", "$2a$11$x1fZNHZb56UDOT6G50O/Y.g8GcHo7lQ/voCe6DoBpAp55bLwjoD4S", null, null, null, 2, "Mr", null, null, new DateTime(2024, 3, 8, 15, 40, 46, 380, DateTimeKind.Unspecified).AddTicks(1770) },
                    { 5, new DateTime(2024, 3, 8, 15, 41, 5, 580, DateTimeKind.Unspecified).AddTicks(7190), null, new DateTime(2024, 3, 8, 15, 40, 30, 575, DateTimeKind.Unspecified).AddTicks(1440), new DateTime(2024, 3, 8, 17, 40, 30, 521, DateTimeKind.Unspecified).AddTicks(8190), null, null, null, "antony@shearwatervf.com", "Antony", false, null, null, "Shumba", "$2a$11$x1fZNHZb56UDOT6G50O/Y.g8GcHo7lQ/voCe6DoBpAp55bLwjoD4S", null, null, null, 2, "Mr", null, null, new DateTime(2024, 3, 8, 15, 40, 46, 380, DateTimeKind.Unspecified).AddTicks(1770) },
                    { 6, new DateTime(2024, 4, 12, 15, 19, 53, 900, DateTimeKind.Unspecified).AddTicks(6273), null, new DateTime(2024, 4, 12, 15, 14, 46, 134, DateTimeKind.Unspecified).AddTicks(5649), new DateTime(2024, 4, 12, 15, 19, 53, 900, DateTimeKind.Unspecified).AddTicks(7119), null, null, null, "irene@shearwatervf.com", "Irene", false, new DateTime(2024, 4, 12, 15, 19, 53, 900, DateTimeKind.Unspecified).AddTicks(7964), null, "Manyowa", "$2a$11$UjbDIp23q1XIJDkUXYT7peF.9bhReAzR0/is3WRaoHUsSy2Bz86o6", null, null, null, 1, "Ms", null, null, new DateTime(2024, 3, 8, 15, 40, 46, 380, DateTimeKind.Unspecified).AddTicks(1770) },
                    { 7, new DateTime(2024, 4, 20, 15, 50, 43, 216, DateTimeKind.Unspecified).AddTicks(6667), null, new DateTime(2024, 4, 20, 15, 50, 43, 216, DateTimeKind.Unspecified).AddTicks(6667), new DateTime(2024, 4, 20, 15, 50, 43, 216, DateTimeKind.Unspecified).AddTicks(6667), null, null, null, "robert@shearwatervf.com", "Robert", false, new DateTime(2024, 4, 20, 15, 50, 43, 216, DateTimeKind.Unspecified).AddTicks(6667), null, "Gororo", "$2a$11$x1fZNHZb56UDOT6G50O/Y.g8GcHo7lQ/voCe6DoBpAp55bLwjoD4S", null, null, null, 2, "Mr", new DateTime(2024, 4, 20, 15, 50, 43, 216, DateTimeKind.Unspecified).AddTicks(6667), null, new DateTime(2024, 4, 20, 15, 50, 43, 216, DateTimeKind.Unspecified).AddTicks(6667) },
                    { 8, new DateTime(2024, 4, 20, 15, 54, 40, 323, DateTimeKind.Unspecified).AddTicks(3333), null, new DateTime(2024, 4, 20, 15, 54, 40, 323, DateTimeKind.Unspecified).AddTicks(3333), new DateTime(2024, 4, 20, 15, 54, 40, 323, DateTimeKind.Unspecified).AddTicks(3333), null, null, null, "explorerscafe@shearwatervf.com", "Antony", false, new DateTime(2024, 4, 20, 15, 54, 40, 323, DateTimeKind.Unspecified).AddTicks(3333), null, "Shumba", "$2a$11$x1fZNHZb56UDOT6G50O/Y.g8GcHo7lQ/voCe6DoBpAp55bLwjoD4S", null, null, null, 2, "Mr", new DateTime(2024, 4, 20, 15, 54, 40, 323, DateTimeKind.Unspecified).AddTicks(3333), null, new DateTime(2024, 4, 20, 15, 54, 40, 323, DateTimeKind.Unspecified).AddTicks(3333) },
                    { 10, new DateTime(2024, 4, 23, 8, 36, 33, 586, DateTimeKind.Unspecified).AddTicks(7408), null, new DateTime(2024, 4, 23, 8, 32, 44, 700, DateTimeKind.Unspecified).AddTicks(2283), new DateTime(2024, 4, 23, 8, 36, 33, 586, DateTimeKind.Unspecified).AddTicks(8274), null, null, null, "mthokozisi@shearwatervf.com", "Mthokozisi", false, new DateTime(2024, 4, 23, 8, 36, 33, 586, DateTimeKind.Unspecified).AddTicks(9155), null, "Nkomazana", "$2a$11$DgdQk6BZdT5Eg9xCpHdZsu9aTmKh2GLlITWPOYFx7I7BViOdaFKCK", null, null, null, 1, "Mr", null, null, new DateTime(2024, 4, 23, 8, 36, 33, 586, DateTimeKind.Unspecified).AddTicks(7408) },
                    { 11, new DateTime(2024, 4, 24, 8, 59, 3, 676, DateTimeKind.Unspecified).AddTicks(1502), null, new DateTime(2024, 4, 24, 8, 58, 23, 696, DateTimeKind.Unspecified).AddTicks(5660), new DateTime(2024, 4, 24, 8, 59, 3, 676, DateTimeKind.Unspecified).AddTicks(1519), null, null, null, "solomon@shearwatervf.com", "Solomon", false, new DateTime(2024, 4, 24, 8, 59, 3, 676, DateTimeKind.Unspecified).AddTicks(1522), null, "Sai", "$2a$11$dTkS7icWQ6lf05qeqfGWOu5ARGNyhBGdRGTNVlJWNriP2PjdKnsNS", null, null, null, 1, "Mr", null, null, new DateTime(2024, 4, 24, 8, 59, 3, 676, DateTimeKind.Unspecified).AddTicks(1502) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CreatorId",
                table: "Accounts",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_DeleterId",
                table: "Accounts",
                column: "DeleterId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LastModifierUserId",
                table: "Accounts",
                column: "LastModifierUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
