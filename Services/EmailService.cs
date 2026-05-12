using System.Net;
using System.Net.Mail;

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
                var host = _cfg["EmailSettings:SmtpHost"]!;
                var port = int.Parse(_cfg["EmailSettings:SmtpPort"]!);
                var kullanici = _cfg["EmailSettings:KullaniciAdi"]!;
                var sifre = _cfg["EmailSettings:Sifre"]!;
                var gonderenAd = _cfg["EmailSettings:GonderenAdi"]!;
                var gonderenEmail = _cfg["EmailSettings:GonderenEmail"]!;

                var mesaj = new MailMessage
                {
                    From = new MailAddress(gonderenEmail, gonderenAd),
                    Subject = konu,
                    Body = icerik,
                    IsBodyHtml = true
                };
                mesaj.To.Add(new MailAddress(aliciEmail, aliciAd));

                using var smtp = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(kullanici, sifre),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtp.SendMailAsync(mesaj);
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
                var sonuc = await GonderAsync(email, ad, konu, icerik);
                if (sonuc) basarili++;
                else basarisiz++;

                // Rate limit için kısa bekleme
                await Task.Delay(200);
            }

            return (basarili, basarisiz);
        }
    }
}