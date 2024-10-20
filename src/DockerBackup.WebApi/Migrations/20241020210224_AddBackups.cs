using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockerBackup.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBackups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContainerBackups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContainerName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerBackups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileBackups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContainerBackupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    ContainerPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileBackups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileBackups_ContainerBackups_ContainerBackupId",
                        column: x => x.ContainerBackupId,
                        principalTable: "ContainerBackups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileBackups_ContainerBackupId",
                table: "FileBackups",
                column: "ContainerBackupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileBackups");

            migrationBuilder.DropTable(
                name: "ContainerBackups");
        }
    }
}
