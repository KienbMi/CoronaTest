using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoronaTest.Persistence.Migrations
{
    public partial class AddModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParticipantId",
                table: "VerificationTokens",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Firstname = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    SocialSecurityNumber = table.Column<string>(nullable: true),
                    Birthdate = table.Column<DateTime>(nullable: false),
                    Mobilephone = table.Column<string>(nullable: true),
                    Postalcode = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    HouseNumber = table.Column<string>(nullable: true),
                    Stair = table.Column<string>(nullable: true),
                    Door = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Examinations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CampaignId = table.Column<int>(nullable: true),
                    ParticipantId = table.Column<int>(nullable: true),
                    TestCenterId = table.Column<int>(nullable: true),
                    Result = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    ExaminationAt = table.Column<DateTime>(nullable: false),
                    Identifier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Examinations_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestCenters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Postalcode = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    SlotCapacity = table.Column<int>(nullable: false),
                    CampaignId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    TestCenterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_TestCenters_TestCenterId",
                        column: x => x.TestCenterId,
                        principalTable: "TestCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTokens_ParticipantId",
                table: "VerificationTokens",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_TestCenterId",
                table: "Campaigns",
                column: "TestCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_CampaignId",
                table: "Examinations",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_ParticipantId",
                table: "Examinations",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_TestCenterId",
                table: "Examinations",
                column: "TestCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCenters_CampaignId",
                table: "TestCenters",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationTokens_Participants_ParticipantId",
                table: "VerificationTokens",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Examinations_TestCenters_TestCenterId",
                table: "Examinations",
                column: "TestCenterId",
                principalTable: "TestCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Examinations_Campaigns_CampaignId",
                table: "Examinations",
                column: "CampaignId",
                principalTable: "Campaigns",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationTokens_Participants_ParticipantId",
                table: "VerificationTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_TestCenters_TestCenterId",
                table: "Campaigns");

            migrationBuilder.DropTable(
                name: "Examinations");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "TestCenters");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_VerificationTokens_ParticipantId",
                table: "VerificationTokens");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "VerificationTokens");
        }
    }
}
