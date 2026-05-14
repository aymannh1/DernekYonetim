using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DernekYonetim.Services
{
    public class EmailService
    {
        private readonly IConfiguration _cfg;

        public EmailService(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task<bool> GonderAsync(
            string aliciEmail,
            string aliciAd,
            string konu,
            string icerik)
        {
            try
            {
                var host          = _cfg["EmailSettings:SmtpHost"]!;
                var port          = int.Parse(_cfg["EmailSettings:SmtpPort"]!);
                var kullanici     = _cfg["EmailSettings:KullaniciAdi"]!;
                var sifre         = _cfg["EmailSettings:Sifre"]!;
                var gonderenAd    = _cfg["EmailSettings:GonderenAdi"]!;
                var gonderenEmail = _cfg["EmailSettings:GonderenEmail"]!;

                var mesaj = new MimeMessage();
                mesaj.From.Add(new MailboxAddress(gonderenAd, gonderenEmail));
                mesaj.To.Add(new MailboxAddress(aliciAd, aliciEmail));
                mesaj.Subject = konu;
                mesaj.Body = new TextPart("html") { Text = icerik };

                using var smtp = new SmtpClient();
                using var cts  = new CancellationTokenSource(TimeSpan.FromSeconds(25));

                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls, cts.Token);
                await smtp.AuthenticateAsync(kullanici, sifre, cts.Token);
                await smtp.SendAsync(mesaj, cts.Token);
                await smtp.DisconnectAsync(true, cts.Token);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(int Basarili, int Basarisiz)> TopluGonderAsync(
            List<(string Email, string Ad)> alicilar,
            string konu,
            string icerik)
        {
            int basarili = 0, basarisiz = 0;

            foreach (var (email, ad) in alicilar)
            {
                if (string.IsNullOrWhiteSpace(email)) { basarisiz++; continue; }

                var sonuc = await GonderAsync(email, ad, konu, icerik);
                if (sonuc) basarili++;
                else basarisiz++;

                await Task.Delay(100);
            }

            return (basarili, basarisiz);
        }
    }
}
