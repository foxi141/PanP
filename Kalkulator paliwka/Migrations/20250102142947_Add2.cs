using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kalkulator_paliwka.Migrations
{
    /// <inheritdoc />
    public partial class Add2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelData_Vehicles_VehicleId",
                table: "FuelData");

            migrationBuilder.DropIndex(
                name: "IX_FuelData_VehicleId",
                table: "FuelData");

            migrationBuilder.DropColumn(
                name: "username",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "FuelData",
                newName: "Vehicleid");

            migrationBuilder.AddColumn<string>(
                name: "userid",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Vehicleid",
                table: "FuelData",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userid",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "Vehicleid",
                table: "FuelData",
                newName: "VehicleId");

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "VehicleId",
                table: "FuelData",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelData_VehicleId",
                table: "FuelData",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelData_Vehicles_VehicleId",
                table: "FuelData",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
