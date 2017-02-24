using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Q42.ImageResizert
{
  public class ImageController : Controller
  {
    private readonly ImageResizertSettings _storageSettings;

    public ImageController(IOptions<ImageResizertSettings> storageSettings)
    {
      _storageSettings = storageSettings.Value;
      if (_storageSettings.CompressionQuality == 0)
        _storageSettings.CompressionQuality = 75;
    }

    [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Any)]
    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetById(string id, int w = 0, int h = 0, bool cover = false, int quality = 75)
    {
      return await Index(id, w, h, cover, quality);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="cover"></param>    
    /// <returns></returns>
    [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index(string id, int width = 0, int height = 0, bool cover = false, int quality = 75)
    {
      // connect to blobstore
      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageSettings.AzureConnectionString);
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

      // get image container
      string downloadContainerName =  _storageSettings.AssetContainerName;
      CloudBlobContainer downloadContainer = blobClient.GetContainerReference(downloadContainerName);

      // get from cache if exists
      var cacheBlob = downloadContainer.GetBlockBlobReference("imagecache/" + id + "-" + width + "-" + height);
      if (await cacheBlob.ExistsAsync())
      {
        var stream = new MemoryStream();
        await cacheBlob.DownloadToStreamAsync(stream);
        return File(stream.ToArray(), "image/jpeg");
      }

      using (Stream stream = new MemoryStream())
      {
        // get original image
        var blob = downloadContainer.GetBlobReference(id);
        if (!await blob.ExistsAsync())
          return BadRequest("File doesn't exist");

        // download image
        await blob.DownloadToStreamAsync(stream);

        // Read from stream.
        using (MagickImage image = new MagickImage(stream))
        {
          // never stretch image
          width = Math.Min(width, image.Width);
          height = Math.Min(height, image.Height);

          image.Quality = quality;
          image.Format = MagickFormat.Jpeg;

          MagickGeometry size = new MagickGeometry(width, height);

          if (cover)
          {
            if (width == 0 || height == 0)
              return BadRequest("Width and height required for cover");

            // convert $input -resize '$widthx$height^' -gravity center -crop '$widthx$height+0+0' $output
            var isWider = (float)image.Width / (float)image.Height > (float)width / (float)height;

            if (isWider)
            {
              image.Resize(0, height);
              image.Crop(width == 0 ? image.Width : width, height, Gravity.Center);
            }
            else
            {
              image.Resize(width, 0);
              image.Crop(width, height == 0 ? image.Height : height, Gravity.Center);
            }
          }
          else
          {
            image.Resize(size);
          }

          byte[] bytes = image.ToByteArray();

          // store in cache
          await cacheBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

          return File(bytes, "image/jpeg");
        }
      }

      throw new Exception("You did it wrong. Find help. Or don't.");
    }
  }
}
