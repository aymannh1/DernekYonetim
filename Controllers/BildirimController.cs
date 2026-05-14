using DernekYonetim.Models;
using DernekYonetim.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DernekYonetim.Controllers
{
    [Authorize]
    public class BildirimController : Controller
    {
        private readonly BildirimService _bildirimService;
        private readonly UserManager<Uye> _userManager;

        public BildirimController(BildirimService bildirimService, UserManager<Uye> userManager)
        {
            _bildirimService = bildirimService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            await _bildirimService.TumunuOkunduIsaretleAsync(userId);
            var liste = await _bildirimService.ListeleAsync(userId);
            return View(liste);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            await _bildirimService.OkunduIsaretleAsync(id, userId);
            return RedirectToAction("Index");
        }
    }
}
