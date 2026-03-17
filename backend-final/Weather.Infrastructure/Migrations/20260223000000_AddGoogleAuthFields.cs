using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Infrastructure.Migrations
{
    public partial class AddGoogleAuthFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make Password nullable (Google users won't have one)
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginProvider",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Local");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "GoogleId", table: "Users");
            migrationBuilder.DropColumn(name: "FullName", table: "Users");
            migrationBuilder.DropColumn(name: "ProfilePicture", table: "Users");
            migrationBuilder.DropColumn(name: "LoginProvider", table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
