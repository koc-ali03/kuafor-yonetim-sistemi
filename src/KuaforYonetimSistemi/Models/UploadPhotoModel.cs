using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace KuaforYonetimSistemi.Pages
{
    public class UploadPhotoModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UploadPhotoModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public IFormFile Photo { get; set; }
        public string ImagePreview { get; set; }
        public string HairSuggestionImage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Photo == null || Photo.Length == 0)
            {
                ModelState.AddModelError("Photo", "Fotoðraf yüklenmedi.");
                return Page();
            }

            using (var content = new MultipartFormDataContent())
            {
                var streamContent = new StreamContent(Photo.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(Photo.ContentType);
                content.Add(streamContent, "file", Photo.FileName);

                var response = await _httpClient.PostAsync("api/hair-suggestion", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    HairSuggestionImage = result;
                    var buffer = new byte[Photo.Length];
                    await Photo.OpenReadStream().ReadAsync(buffer);
                    ImagePreview = $"data:{Photo.ContentType};base64,{Convert.ToBase64String(buffer)}";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Saç önerisi alýnamadý.");
                }
            }

            return Page();
        }
    }
}
