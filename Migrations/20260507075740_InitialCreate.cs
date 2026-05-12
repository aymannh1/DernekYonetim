using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DernekYonetim.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Basvurular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TcKimlik = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meslek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BelgeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BasvuruNotu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    BasvuruTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IslemYapanId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedNedeni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EkBilgiMesaji = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basvurular", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uyeler",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UyeNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TcKimlik = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meslek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilFotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UyelikDurumu = table.Column<int>(type: "int", nullable: false),
                    UyelikTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonGiris = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uyeler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aidatlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UyeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Donem = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    SonOdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdemeDurumu = table.Column<int>(type: "int", nullable: false),
                    OdemeYontemi = table.Column<int>(type: "int", nullable: true),
                    DekontUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DekontOnaylandi = table.Column<bool>(type: "bit", nullable: false),
                    OnaylayanId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aidatlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aidatlar_Uyeler_UyeId",
                        column: x => x.UyeId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Uyeler_UserId",
                        column: x => x.UserId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Uyeler_UserId",
                        column: x => x.UserId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Uyeler_UserId",
                        column: x => x.UserId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Uyeler_UserId",
                        column: x => x.UserId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Belgeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DosyaYolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DosyaAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DosyaTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DosyaBoyutu = table.Column<long>(type: "bigint", nullable: false),
                    Versiyon = table.Column<int>(type: "int", nullable: false),
                    OncekiVersiyonId = table.Column<int>(type: "int", nullable: true),
                    ErisimSeviyesi = table.Column<int>(type: "int", nullable: false),
                    YukleyenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    YuklemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Belgeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Belgeler_Uyeler_YukleyenId",
                        column: x => x.YukleyenId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UyeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false),
                    YonlendirmeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bildirimler_Uyeler_UyeId",
                        column: x => x.UyeId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Duyurular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ozet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hedef = table.Column<int>(type: "int", nullable: false),
                    KamuoyunaAcik = table.Column<bool>(type: "bit", nullable: false),
                    YayinTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    OlusturanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duyurular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Duyurular_Uyeler_OlusturanId",
                        column: x => x.OlusturanId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Etkinlikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kategori = table.Column<int>(type: "int", nullable: false),
                    Yer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kontenjan = table.Column<int>(type: "int", nullable: true),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UcretsizMi = table.Column<bool>(type: "bit", nullable: false),
                    Ucret = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    OlusturanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etkinlikler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Etkinlikler_Uyeler_OlusturanId",
                        column: x => x.OlusturanId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UyeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Olay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAdresi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogKayitlari_Uyeler_UyeId",
                        column: x => x.UyeId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MaliHareketler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BelgeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydEdenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaliHareketler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaliHareketler_Uyeler_KaydEdenId",
                        column: x => x.KaydEdenId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EtkinlikKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UyeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EtkinlikId = table.Column<int>(type: "int", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    KatildiMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtkinlikKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EtkinlikKayitlari_Etkinlikler_EtkinlikId",
                        column: x => x.EtkinlikId,
                        principalTable: "Etkinlikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EtkinlikKayitlari_Uyeler_UyeId",
                        column: x => x.UyeId,
                        principalTable: "Uyeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aidatlar_UyeId_Donem",
                table: "Aidatlar",
                columns: new[] { "UyeId", "Donem" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Belgeler_YukleyenId",
                table: "Belgeler",
                column: "YukleyenId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_UyeId",
                table: "Bildirimler",
                column: "UyeId");

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_OlusturanId",
                table: "Duyurular",
                column: "OlusturanId");

            migrationBuilder.CreateIndex(
                name: "IX_EtkinlikKayitlari_EtkinlikId",
                table: "EtkinlikKayitlari",
                column: "EtkinlikId");

            migrationBuilder.CreateIndex(
                name: "IX_EtkinlikKayitlari_UyeId_EtkinlikId",
                table: "EtkinlikKayitlari",
                columns: new[] { "UyeId", "EtkinlikId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_OlusturanId",
                table: "Etkinlikler",
                column: "OlusturanId");

            migrationBuilder.CreateIndex(
                name: "IX_LogKayitlari_UyeId",
                table: "LogKayitlari",
                column: "UyeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaliHareketler_KaydEdenId",
                table: "MaliHareketler",
                column: "KaydEdenId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Uyeler",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Uyeler_UyeNo",
                table: "Uyeler",
                column: "UyeNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Uyeler",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aidatlar");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Basvurular");

            migrationBuilder.DropTable(
                name: "Belgeler");

            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "Duyurular");

            migrationBuilder.DropTable(
                name: "EtkinlikKayitlari");

            migrationBuilder.DropTable(
                name: "LogKayitlari");

            migrationBuilder.DropTable(
                name: "MaliHareketler");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Etkinlikler");

            migrationBuilder.DropTable(
                name: "Uyeler");
        }
    }
}
