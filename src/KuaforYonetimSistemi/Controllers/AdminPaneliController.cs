using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace KuaforYonetimSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPaneliController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            // Copy cookies from the current user's context
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                handler.CookieContainer.Add(new Uri("https://localhost:7085"), new System.Net.Cookie(cookie.Key, cookie.Value));
            }

            var httpClient = new HttpClient(handler);

            // Fetch data from the REST API endpoints
            var enPopulerServislerJson = await httpClient.GetAsync("https://localhost:7085/api/AdminPaneliApi/EnPopulerServisler");
            var gunlukRandevuSayisiJson = await httpClient.GetAsync("https://localhost:7085/api/AdminPaneliApi/GunlukRandevuSayisi");
            var onaylanmamisRandevularJson = await httpClient.GetAsync("https://localhost:7085/api/AdminPaneliApi/OnaylanmamisRandevular");

            var enPopulerServisler = JsonConvert.DeserializeObject<List<dynamic>>(await enPopulerServislerJson.Content.ReadAsStringAsync());
            var gunlukRandevuSayisi = JsonConvert.DeserializeObject<List<dynamic>>(await gunlukRandevuSayisiJson.Content.ReadAsStringAsync());
            var onaylanmamisRandevular = JsonConvert.DeserializeObject<List<dynamic>>(await onaylanmamisRandevularJson.Content.ReadAsStringAsync());

            ViewData["EnPopulerServisler"] = enPopulerServisler;
            ViewData["GunlukRandevuSayisi"] = gunlukRandevuSayisi;
            ViewData["OnaylanmamisRandevular"] = onaylanmamisRandevular;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RandevuOnayla(int id)
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            // Copy cookies from the current user's context
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                handler.CookieContainer.Add(new Uri("https://localhost:7085"), new System.Net.Cookie(cookie.Key, cookie.Value));
            }

            var httpClient = new HttpClient(handler);

            var response = await httpClient.PostAsync($"https://localhost:7085/api/AdminPaneliApi/RandevuOnayla/{id}", null);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = $"Randevu ID {id} başarıyla onaylandı.";
            else
                TempData["ErrorMessage"] = $"Randevu ID {id} onaylanamadı.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RandevuSil(int id)
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            // Copy cookies from the current user's context
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                handler.CookieContainer.Add(new Uri("https://localhost:7085"), new System.Net.Cookie(cookie.Key, cookie.Value));
            }

            var httpClient = new HttpClient(handler);

            var response = await httpClient.PostAsync($"https://localhost:7085/api/AdminPaneliApi/RandevuSil/{id}", null);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = $"Randevu ID {id} başarıyla silindi.";
            else
                TempData["ErrorMessage"] = $"Randevu ID {id} silinemedi.";

            return RedirectToAction("Index");
        }
    }
}
