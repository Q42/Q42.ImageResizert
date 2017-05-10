using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Q42.ImageResizert
{
    public class ImageController : Controller
    {
        private readonly ImageResizertService imageService;

        public ImageController(IOptions<ImageResizertSettings> settings)
        {
            imageService = new ImageResizertService(settings.Value);
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Any)]
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetById(string id, int? w = null, int? h = null, bool cover = false, int? quality = null)
        {
            try
            {
                var image = await imageService.GetImageAsync(id, w, h, cover, quality);

                return File(image, "image/jpeg");
            }
            catch (ArgumentException error)
            {
                return BadRequest(error.Message);
            }
            catch (AssetNotFoundException error)
            {
                return BadRequest(error.Message);
            }
            catch (Exception error)
            {
                throw error;
            }
        }
    }
}
