using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DernekYonetim.Migrations
{
    /// <inheritdoc />
    public partial class ProjeModulu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kategori = table.Column<int>(type: "int", nullable: false),
                    DigerKategori = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ulke = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sehir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HedefBütce = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    ToplamBagis = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KapakFotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projeler_Uyeler_OlusturanId",
                        column: x => x.OlusturanId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjeBelgeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjeId = table.Column<int>(type: "int", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DosyaYoluVeyaLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KucukResimUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DosyaBoyutu = table.Column<long>(type: "bigint", nullable: true),
                    YukleyenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    YuklemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjeBelgeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjeBelgeleri_Projeler_ProjeId",
                        column: x => x.ProjeId,
                        principalTable: "Projeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjeBelgeleri_Uyeler_YukleyenId",
                        column: x => x.YukleyenId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjeGuncellemeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjeId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IlerlemeYuzdesi = table.Column<int>(type: "int", nullable: false),
                    YazanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjeGuncellemeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjeGuncellemeleri_Projeler_ProjeId",
                        column: x => x.ProjeId,
                        principalTable: "Projeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjeGuncellemeleri_Uyeler_YazanId",
                        column: x => x.YazanId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjeBelgeleri_ProjeId",
                table: "ProjeBelgeleri",
                column: "ProjeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeBelgeleri_YukleyenId",
                table: "ProjeBelgeleri",
                column: "YukleyenId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGuncellemeleri_ProjeId",
                table: "ProjeGuncellemeleri",
                column: "ProjeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjeGuncellemeleri_YazanId",
                table: "ProjeGuncellemeleri",
                column: "YazanId");

            migrationBuilder.CreateIndex(
                name: "IX_Projeler_OlusturanId",
                table: "Projeler",
                column: "OlusturanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjeBelgeleri");

            migrationBuilder.DropTable(
                name: "ProjeGuncellemeleri");

            migrationBuilder.DropTable(
                name: "Projeler");
        }
    }
}
