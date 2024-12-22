
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KuaforYonetimSistemi.Models
{

    public class HairstyleChangerService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://www.ailabapi.com/api/portrait/effects/hairstyle-editor";
        private const string ApiKey = "81h0o46u9JdonxzVkO7Bjm3KviWablHTGLWkpC6S07rI3L8yPsP9OiKEqASeDXwI";
        private const string ApiHost = "hairstyle-changer.p.rapidapi.com";

        public HairstyleChangerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", ApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", ApiHost);
        }

        public async Task<string> ChangeHairstyleAsync(byte[] fileBytes, string fileName, string hairstyle)
        {
            // Dosyayı Base64'e çevir
            string base64Image = Convert.ToBase64String(fileBytes);

            // API'ye gönderilecek veriler
            var requestData = new
            {
                image = base64Image,
                hairstyle = hairstyle
            };

            // JSON'a dönüştür
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API çağrısı yap
            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }

            // Yanıtı işle
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonConvert.DeserializeObject(responseString);

            // Görsel Base64 formatında döndürülüyor
            return responseData?.data?.image;
        }
    }

}
