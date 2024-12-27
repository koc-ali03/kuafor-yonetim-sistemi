using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Collections.Generic;

namespace KuaforYonetimSistemi.Controllers
{
    public class HairstyleController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _uploadFolder;

        public HairstyleController(IHttpClientFactory clientFactory, IWebHostEnvironment env)
        {
            _clientFactory = clientFactory;
            _uploadFolder = Path.Combine(env.WebRootPath, "uploads");

            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        public IActionResult Index()
        {
            // Saç tipi kodlarý ve isimleri için bir sözlük oluþturun
            var hairTypeNames = new Dictionary<string, string>
            {
                { "101", "Bangs" },
                { "201", "Long hair" },
                { "301", "Bangs with long hair" },
                { "401", "Medium hair increase" },
                { "402", "Light hair increase" },
                { "403", "Heavy hair increase" },
                { "502", "Light curling" },
                { "503", "Heavy curling" },
                { "603", "Short hair" },
                { "801", "Blonde" },
                { "901", "Straight hair" },
                { "1001", "Oil-free hair" },
                { "1101", "Hairline fill" },
                { "1201", "Smooth hair" },
                { "1301", "Fill hair gap" }
            };
            ViewBag.HairTypeNames = hairTypeNames;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadHairstyle(IFormFile imageFile, string hairType1, string hairType2, string hairType3)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ViewBag.Error = "Lütfen bir resim dosyasý seçin.";
                return View("Index");
            }

            // Resim gereksinimlerini kontrol et
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                ViewBag.Error = "Resim boyutu 5 MB'dan büyük olamaz.";
                return View("Index");
            }

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (extension != ".jpeg" && extension != ".jpg" && extension != ".png" && extension != ".bmp")
            {
                ViewBag.Error = "Resim formatý JPEG, JPG, PNG veya BMP olmalýdýr.";
                return View("Index");
            }

            var tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            using (var image = System.Drawing.Image.FromFile(tempFilePath))
            {
                if (image.Width > 4096 || image.Height > 4096)
                {
                    ViewBag.Error = "Resim çözünürlüðü 4096x4096 pikselden büyük olamaz.";
                    System.IO.File.Delete(tempFilePath);
                    return View("Index");
                }
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(_uploadFolder, fileName);

            System.IO.File.Copy(tempFilePath, filePath);
            System.IO.File.Delete(tempFilePath);

            // Seçilen saç tiplerini kullanarak API isteklerini hazýrla
            var hairTypes = new List<string> { hairType1, hairType2, hairType3 };
            var client = _clientFactory.CreateClient();
            var tasks = new List<Task<string>>();

            foreach (var hairType in hairTypes)
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://hairstyle-changer.p.rapidapi.com/huoshan/facebody/hairstyle"),
                    Headers =
                    {
                        { "x-rapidapi-key", "6a2b428e00msh4a46f9d648158ddp1112d7jsn27be06f9a950" },
                        { "x-rapidapi-host", "hairstyle-changer.p.rapidapi.com" },
                    },
                    Content = new MultipartFormDataContent
                    {
                        { new StreamContent(System.IO.File.OpenRead(filePath)), "image_target", fileName },
                        { new StringContent(hairType), "hair_type" }
                    }
                };
                tasks.Add(SendApiRequestAndGetBase64(client, request));
            }

            await Task.WhenAll(tasks);

            ViewBag.ResultImagesBase64 = tasks.Select(t => t.Result).ToList();
            ViewBag.SelectedHairTypes = hairTypes; // Seçilen saç tiplerini de ViewBag'e ekle
            ViewBag.UploadedImagePath = "/uploads/" + fileName;

            return View("Result");
        }

        private async Task<string> SendApiRequestAndGetBase64(HttpClient client, HttpRequestMessage request)
        {
            try
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);
                    return apiResponse.data.image;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API isteði hatasý: {ex.Message}");
                return null;
            }
        }
    }
}