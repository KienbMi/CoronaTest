using Microsoft.EntityFrameworkCore.Migrations;

namespace CoronaTest.Persistence.Migrations
{
    public partial class manyToManyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_TestCenters_TestCenterId",
                table: "Campaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCenters_Campaigns_CampaignId",
                table: "TestCenters");

            migrationBuilder.DropIndex(
                name: "IX_TestCenters_CampaignId",
                table: "TestCenters");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_TestCenterId",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "TestCenters");

            migrationBuilder.DropColumn(
                name: "TestCenterId",
                table: "Campaigns");

            migrationBuilder.CreateTable(
                name: "CampaignTestCenter",
                columns: table => new
                {
                    AvailableInCampaignsId = table.Column<int>(type: "int", nullable: false),
                    AvailableTestCentersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTestCenter", x => new { x.AvailableInCampaignsId, x.AvailableTestCentersId });
                    table.ForeignKey(
                        name: "FK_CampaignTestCenter_Campaigns_AvailableInCampaignsId",
                        column: x => x.AvailableInCampaignsId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignTestCenter_TestCenters_AvailableTestCentersId",
                        column: x => x.AvailableTestCentersId,
                        principalTable: "TestCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTestCenter_AvailableTestCentersId",
                table: "CampaignTestCenter",
                column: "AvailableTestCentersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignTestCenter");

            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "TestCenters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestCenterId",
                table: "Campaigns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCenters_CampaignId",
                table: "TestCenters",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_TestCenterId",
                table: "Campaigns",
                column: "TestCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campaigns_TestCenters_TestCenterId",
                table: "Campaigns",
                column: "TestCenterId",
                principalTable: "TestCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCenters_Campaigns_CampaignId",
                table: "TestCenters",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
