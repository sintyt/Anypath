using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fullpath",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Current = table.Column<string>(type: "TEXT", nullable: false),
                    Justice = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fullpath", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FolderId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Memo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Fullpath_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Fullpath",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobDb",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FolderId = table.Column<string>(type: "TEXT", nullable: true),
                    Start = table.Column<string>(type: "TEXT", nullable: false),
                    Finish = table.Column<string>(type: "TEXT", nullable: true),
                    Company = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    JobFileId = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    Mark = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobDb_Fullpath_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Fullpath",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobDb_Fullpath_JobFileId",
                        column: x => x.JobFileId,
                        principalTable: "Fullpath",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FolderId = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    Birth = table.Column<string>(type: "TEXT", nullable: true),
                    Zipcode = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    CompanyId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Member_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Member_Fullpath_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Fullpath",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_FolderId",
                table: "Companies",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDb_FolderId",
                table: "JobDb",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDb_JobFileId",
                table: "JobDb",
                column: "JobFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_CompanyId",
                table: "Member",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_FolderId",
                table: "Member",
                column: "FolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobDb");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Fullpath");
        }
    }
}
