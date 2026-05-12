using DernekYonetim.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DernekYonetim.Data
{
    public class AppDbContext : IdentityDbContext<Uye>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ── DbSet'ler ──────────────────────────────────────────────────
        public DbSet<Basvuru> Basvurular { get; set; }
        public DbSet<Aidat> Aidatlar { get; set; }
        public DbSet<Etkinlik> Etkinlikler { get; set; }
        public DbSet<EtkinlikKayit> EtkinlikKayitlari { get; set; }
        public DbSet<Duyuru> Duyurular { get; set; }
        public DbSet<MaliHareket> MaliHareketler { get; set; }
        public DbSet<Belge> Belgeler { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<LogKaydi> LogKayitlari { get; set; }
        public DbSet<Proje> Projeler { get; set; }
        public DbSet<ProjeBelge> ProjeBelgeleri { get; set; }
        public DbSet<ProjeGuncelleme> ProjeGuncellemeleri { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── TABLO ADLARI ───────────────────────────────────────────
            builder.Entity<Uye>().ToTable("Uyeler");

            // ── UYE ───────────────────────────────────────────────────
            builder.Entity<Uye>(e =>
            {
                e.Property(u => u.Ad).HasMaxLength(100).IsRequired();
                e.Property(u => u.Soyad).HasMaxLength(100).IsRequired();
                e.Property(u => u.UyeNo).HasMaxLength(20);
                e.Property(u => u.Telefon).HasMaxLength(20);
                e.Property(u => u.TcKimlik).HasMaxLength(500); // Şifreli
                e.HasIndex(u => u.UyeNo).IsUnique();
            });

            // ── AIDAT ─────────────────────────────────────────────────
            builder.Entity<Aidat>(e =>
            {
                e.Property(a => a.Tutar).HasPrecision(10, 2);
                e.Property(a => a.Donem).HasMaxLength(10);

                e.HasOne(a => a.Uye)
                 .WithMany(u => u.Aidatlar)
                 .HasForeignKey(a => a.UyeId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Aynı üye, aynı dönemde yalnızca 1 aidat kaydı
                e.HasIndex(a => new { a.UyeId, a.Donem }).IsUnique();
            });

            // ── ETKİNLİK ─────────────────────────────────────────────
            builder.Entity<Etkinlik>(e =>
            {
                e.Property(et => et.Baslik).HasMaxLength(255).IsRequired();
                e.Property(et => et.Ucret).HasPrecision(10, 2);

                e.HasOne(et => et.Olusturan)
                 .WithMany()
                 .HasForeignKey(et => et.OlusturanId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── ETKİNLİK KAYIT ────────────────────────────────────────
            builder.Entity<EtkinlikKayit>(e =>
            {
                e.HasOne(ek => ek.Uye)
                 .WithMany(u => u.EtkinlikKayitlari)
                 .HasForeignKey(ek => ek.UyeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(ek => ek.Etkinlik)
                 .WithMany(et => et.Kayitlar)
                 .HasForeignKey(ek => ek.EtkinlikId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Bir üye bir etkinliğe yalnızca 1 kez kayıt olabilir
                e.HasIndex(ek => new { ek.UyeId, ek.EtkinlikId }).IsUnique();
            });

            // ── DUYURU ────────────────────────────────────────────────
            builder.Entity<Duyuru>(e =>
            {
                e.Property(d => d.Baslik).HasMaxLength(255).IsRequired();

                e.HasOne(d => d.Olusturan)
                 .WithMany()
                 .HasForeignKey(d => d.OlusturanId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── MALİ HAREKET ──────────────────────────────────────────
            builder.Entity<MaliHareket>(e =>
            {
                e.Property(m => m.Tutar).HasPrecision(10, 2);

                e.HasOne(m => m.KaydEden)
                 .WithMany()
                 .HasForeignKey(m => m.KaydEdenId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── BELGE ─────────────────────────────────────────────────
            builder.Entity<Belge>(e =>
            {
                e.HasOne(b => b.Yukleyen)
                 .WithMany(u => u.Belgeler)
                 .HasForeignKey(b => b.YukleyenId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── BİLDİRİM ─────────────────────────────────────────────
            builder.Entity<Bildirim>(e =>
            {
                e.HasOne(b => b.Uye)
                 .WithMany(u => u.Bildirimler)
                 .HasForeignKey(b => b.UyeId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── LOG KAYDI ─────────────────────────────────────────────
            builder.Entity<LogKaydi>(e =>
            {
                e.HasOne(l => l.Uye)
                 .WithMany()
                 .HasForeignKey(l => l.UyeId)
                 .OnDelete(DeleteBehavior.SetNull);
            });
            // ── PROJE ─────────────────────────────────────────────────────
            builder.Entity<Proje>(e =>
            {
                e.Property(p => p.Baslik).HasMaxLength(255).IsRequired();
                e.Property(p => p.HedefBütce).HasPrecision(12, 2);
                e.Property(p => p.ToplamBagis).HasPrecision(12, 2);

                e.HasOne(p => p.Olusturan)
                 .WithMany()
                 .HasForeignKey(p => p.OlusturanId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── PROJE BELGE ───────────────────────────────────────────────
            builder.Entity<ProjeBelge>(e =>
            {
                e.HasOne(pb => pb.Proje)
                 .WithMany(p => p.Belgeler)
                 .HasForeignKey(pb => pb.ProjeId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(pb => pb.Yukleyen)
                 .WithMany()
                 .HasForeignKey(pb => pb.YukleyenId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── PROJE GUNCELLEME ──────────────────────────────────────────
            builder.Entity<ProjeGuncelleme>(e =>
            {
                e.HasOne(pg => pg.Proje)
                 .WithMany(p => p.Guncellemeler)
                 .HasForeignKey(pg => pg.ProjeId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(pg => pg.Yazan)
                 .WithMany()
                 .HasForeignKey(pg => pg.YazanId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}