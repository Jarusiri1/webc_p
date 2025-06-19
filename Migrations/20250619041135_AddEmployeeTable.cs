using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applications",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Applications");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "tblPermission");

            migrationBuilder.RenameTable(
                name: "Applications",
                newName: "tblApplication");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tblPermission",
                newName: "ApplicationId");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "tblApplication",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "Telephone",
                table: "tblApplication",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "tblApplication",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "tblApplication",
                newName: "ApplicationStatus");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "tblPermission",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "PermissionId",
                table: "tblPermission",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "tblPermission",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "tblPermission",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PermissionName",
                table: "tblPermission",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                table: "tblPermission",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "tblPermission",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "tblApplication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationId",
                table: "tblApplication",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "tblApplication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                table: "tblApplication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblPermission",
                table: "tblPermission",
                column: "PermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblApplication",
                table: "tblApplication",
                column: "ApplicationId");

            migrationBuilder.CreateTable(
                name: "tblApplicationAdmin",
                columns: table => new
                {
                    ApplicationAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblApplicationAdmin", x => x.ApplicationAdminId);
                });

            migrationBuilder.CreateTable(
                name: "tblEmployee",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEmployee", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "tblGroup",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblGroup", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_tblGroup_tblApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "tblApplication",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserGroup",
                columns: table => new
                {
                    UserGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserGroup", x => x.UserGroupId);
                    table.ForeignKey(
                        name: "FK_tblUserGroup_tblGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "tblGroup",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblGroup_ApplicationId",
                table: "tblGroup",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserGroup_GroupId",
                table: "tblUserGroup",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblApplicationAdmin");

            migrationBuilder.DropTable(
                name: "tblEmployee");

            migrationBuilder.DropTable(
                name: "tblUserGroup");

            migrationBuilder.DropTable(
                name: "tblGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblPermission",
                table: "tblPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblApplication",
                table: "tblApplication");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "tblPermission");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "tblPermission");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "tblPermission");

            migrationBuilder.DropColumn(
                name: "PermissionName",
                table: "tblPermission");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "tblPermission");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "tblPermission");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "tblApplication");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "tblApplication");

            migrationBuilder.RenameTable(
                name: "tblPermission",
                newName: "Permissions");

            migrationBuilder.RenameTable(
                name: "tblApplication",
                newName: "Applications");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "Permissions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Applications",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Applications",
                newName: "Telephone");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Applications",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "ApplicationStatus",
                table: "Applications",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationId",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Applications",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applications",
                table: "Applications",
                column: "Id");
        }
    }
}
