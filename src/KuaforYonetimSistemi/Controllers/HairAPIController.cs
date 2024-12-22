using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KuaforYonetimSistemi.Controllers
{
    public class HairAPIController : Controller
    {
        private readonly ILogger<HairAPIController> _logger;

        public HairAPIController(ILogger<HairAPIController> logger)
        {
            _logger = logger;
        }

        // GET: HairAPI/ChangeHairstyleForm
        public IActionResult ChangeHairstyleForm()
        {
            return View();
        }

        // POST: HairAPI/ChangeHairstyle
        [HttpPost]
        public async Task<IActionResult> ChangeHairstyle(IFormFile uploadedFile, string hairType)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                return Json(new { error = "Lütfen bir resim yükleyin." });
            }

            using (var memoryStream = new MemoryStream())
            {
                await uploadedFile.CopyToAsync(memoryStream);
                string base64Image = Convert.ToBase64String(memoryStream.ToArray());

                // API çaðrýsýný yap
                var apiResult = await CallApiAndGetResult(base64Image, hairType);

                if (!string.IsNullOrEmpty(apiResult))
                {
                    return Json(new { resultImage = apiResult });
                }
                else
                {
                    return Json(new { error = "API iþleminde bir hata oluþtu." });
                }
            }
        }

        private async Task<string> CallApiAndGetResult(string base64Image, string hairType)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.ailabtools.com/api/ai-portrait/effects/hairstyle-editor-pro/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "81h0o46u9JdonxzVkO7Bjm3KviWablHTGLWkpC6S07rI3L8yPsP9OiKEqASeDXwI");

                var requestContent = new MultipartFormDataContent
        {
            { new StringContent(base64Image), "image" },
            { new StringContent(hairType), "styleId" }
        };

                var response = await client.PostAsync("apply", requestContent);

                // Yanýtýn baþarýlý olup olmadýðýný kontrol et
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    // Hata mesajýný logla
                    _logger.LogError($"API Hatasý: {response.StatusCode}, {errorResponse}");
                    return null;
                }

                // Yanýtý iþle
                var responseString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseString);

                if (apiResponse?.data?.image != null)
                {
                    return apiResponse.data.image; // Base64 resim döndür
                }

                return null;
            }
        }

        public class ApiResponse
        {
            public ApiData data { get; set; }
        }

        public class ApiData
        {
            public string image { get; set; }
        }
    }
}
