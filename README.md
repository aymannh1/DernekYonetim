# HABSİS – HAKİD Yönetim Sistemi

**Habesistan Kalkınma ve İşbirliği Derneği** için geliştirilmiş kapsamlı bir dernek yönetim sistemi.

---

## Özellikler

- **Üye Yönetimi** – Online başvuru, üye takibi, durum güncelleme ve profil yönetimi
- **Aidat Takibi** – Otomatik aidat oluşturma, dekont yükleme ve onay sistemi
- **Etkinlik Yönetimi** – Etkinlik duyurusu, online kayıt ve katılımcı takibi
- **Duyurular** – Hedef kitleye göre duyuru paylaşımı
- **Belge Yönetimi** – Kategorili belge arşivi ve erişim kontrolü
- **Proje Takibi** – Habesistan'daki projeleri fotoğraf, video ve raporlarla takip
- **Mali İşlemler** – Gelir/gider kaydı ve aylık mali raporlar
- **Toplu Mail** – Üyelere toplu e-posta gönderimi
- **Rol Bazlı Yetkilendirme** – Admin, Yönetim ve Üye rolleri

---

## Teknoloji Yığını

| Katman | Teknoloji |
|--------|-----------|
| Backend | ASP.NET Core 8 MVC |
| ORM | Entity Framework Core 8 |
| Veritabanı | PostgreSQL (Render) / SQL Server (yerel) |
| Kimlik Doğrulama | ASP.NET Core Identity |
| Arayüz | Bootstrap 5, Bootstrap Icons, Inter Font |
| Deployment | Render (Docker) |

---

## Kurulum (Yerel Geliştirme)

### Gereksinimler
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL veya SQL Server

### Adımlar

```bash
# Repoyu klonla
git clone https://github.com/aymannh1/DernekYonetim.git
cd DernekYonetim

# Bağlantı dizesini güncelle (appsettings.json)
# DefaultConnection → kendi veritabanı bilgilerinizi girin

# Paketleri yükle ve çalıştır
dotnet restore
dotnet run
```

Uygulama ilk çalıştığında veritabanını otomatik oluşturur ve varsayılan admin kullanıcısını oluşturur.

### Varsayılan Admin Girişi
| Alan | Değer |
|------|-------|
| E-posta | `admin@dernekdbs.org` |
| Şifre | `Admin@12345!` |

> İlk girişten sonra şifrenizi değiştirmeniz önerilir.

---

## Deployment (Render)

Proje Render üzerinde Docker ile deploy edilmektedir. `render.yaml` dosyası web servisi ve PostgreSQL veritabanını otomatik olarak yapılandırır.

1. [Render](https://render.com) hesabınıza giriş yapın
2. **New → Blueprint** seçin
3. Bu repoyu bağlayın — `render.yaml` otomatik algılanır
4. **Apply** butonuna tıklayın

---

## Proje Yapısı

```
├── Controllers/        # MVC Controller'lar
├── Data/               # DbContext ve veritabanı seed
├── Migrations/         # EF Core migration dosyaları
├── Models/             # Veri modelleri
├── Services/           # İş mantığı servisleri
├── ViewModels/         # View model sınıfları
├── Views/              # Razor View dosyaları
├── wwwroot/            # Statik dosyalar (CSS, JS)
├── Dockerfile          # Docker build konfigürasyonu
└── render.yaml         # Render deployment konfigürasyonu
```

---

## Lisans

Bu proje HAKİD – Habesistan Kalkınma ve İşbirliği Derneği için geliştirilmiştir.
