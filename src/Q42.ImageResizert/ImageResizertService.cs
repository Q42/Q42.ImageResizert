using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Q42.ImageResizert
{
    public class ImageResizertService
    {
        private readonly CloudBlobContainer downloadContainer;
        private readonly CloudBlobContainer cacheContainer;
        private readonly string cacheFolder;
        private readonly int compressionQuality;

        public ImageResizertService(ImageResizertSettings settings)
        {
            var storageAccount = CloudStorageAccount.Parse(settings.AzureConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            downloadContainer = blobClient.GetContainerReference(settings.AssetContainerName);
            cacheContainer = string.IsNullOrEmpty(settings.CacheContainerName) ? downloadContainer : blobClient.GetContainerReference(settings.CacheContainerName);
            cacheFolder = settings.CacheContainerFolder ?? "imagecache";
            compressionQuality = settings.CompressionQuality;
        }

        public async Task<byte[]> GetImageAsync(string id, int? width = null, int? height = null, bool cover = false, int? quality = null)
        {
            return await GetImageAsync(id, width, height, cover, quality ?? compressionQuality);
        }

        private async Task<byte[]> GetImageAsync(string id, int? width, int? height, bool cover, int quality)
        {
            // get from cache if exists
            var cacheBlob = cacheContainer.GetBlockBlobReference(GetCacheUrl(id, width, height, cover, quality));
            if (await cacheBlob.ExistsAsync())
            {
                var stream = new MemoryStream();
                await cacheBlob.DownloadToStreamAsync(stream);

                return stream.ToArray();
            }

            using (Stream stream = new MemoryStream())
            {
                // get original image
                var blob = downloadContainer.GetBlobReference(id);
                if (!await blob.ExistsAsync())
                    throw new AssetNotFoundException();

                // download image
                await blob.DownloadToStreamAsync(stream);

                // Read from stream.
                using (MagickImage image = new MagickImage(stream))
                {
                    image.Quality = quality;
                    image.Format = MagickFormat.Jpeg;

                    if (cover)
                    {
                        CropImage(image, width, height);
                    }
                    else
                    {
                        ResizeImage(image, width, height);
                    }

                    byte[] bytes = image.ToByteArray();

                    // store in cache
                    await cacheBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

                    return bytes;
                }
            }

            throw new Exception("You did it wrong. Find help. Or don't.");
        }

        private void CropImage(MagickImage image, int? width, int? height)
        {
            if (!width.HasValue || !height.HasValue)
            {
                throw new ArgumentException("Both width and height are required for cover");
            }

            var newWidth = Math.Min(width.Value, image.Width);
            var newHeight = Math.Min(height.Value, image.Height);
            
            // convert $input -resize '$widthx$height^' -gravity center -crop '$widthx$height+0+0' $output
            var isWider = (float)image.Width / (float)image.Height > (float)newWidth / (float)newHeight;
            if (isWider)
            {
                image.Resize(0, newHeight);
            }
            else
            {
                image.Resize(newWidth, 0);
            }

            image.Crop(newWidth, newHeight, Gravity.Center);
        }

        private void ResizeImage(MagickImage image, int? width, int? height)
        {
            var newWidth = Math.Min(width ?? 0, image.Width);
            var newHeight = Math.Min(height ?? 0, image.Height);

            MagickGeometry size = new MagickGeometry(newWidth, newHeight);
            image.Resize(size);
        }

        private string GetCacheUrl(string id, int? width, int? height, bool cover, int quality)
        {
            var filename = string.Format("{0}-{1}-{2}-{3}-{4}", id, width ?? 0, height ?? 0, cover, quality);

            if (!string.IsNullOrEmpty(this.cacheFolder))
            {
                return string.Format("{0}/{1}", cacheFolder, filename);
            }

            return filename;
        }

    }
}
