using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
                using (var stream = new MemoryStream())
                {
                    await cacheBlob.DownloadToStreamAsync(stream);
                    return stream.ToArray();
                }
            }

            using (var stream = new MemoryStream())
            {
                // get original image
                var blob = downloadContainer.GetBlobReference(id);
                if (!await blob.ExistsAsync())
                    throw new AssetNotFoundException();

                if (blob.Properties.Length == 0)
                {
                    throw new AssetInvalidException();
                }

                // download image
                await blob.DownloadToStreamAsync(stream);
                var imageBytes = stream.ToArray();
                

                // Read from stream.
                using (var image = Image.Load(Configuration.Default, imageBytes, out var format))
                {
                    if (cover)
                    {
                        CropImage(image, width, height);
                    }
                    else
                    {
                        ResizeImage(image, width, height);
                    }

                    using (var output = new MemoryStream())
                    {
                        if (format.DefaultMimeType == JpegFormat.Instance.DefaultMimeType)
                        {
                            image.SaveAsJpeg(output, new JpegEncoder
                            {
                                Quality = quality,
                                Subsample = JpegSubsample.Ratio444
                            });
                        }
                        else
                        {
                            image.Save(output, format);
                        }

                        // store in cache

                        var result = output.ToArray();
                        using (var saveStream = new MemoryStream(result))
                        {
                            await cacheBlob.UploadFromStreamAsync(saveStream);
                            cacheBlob.Properties.ContentType = format.DefaultMimeType;
                            await cacheBlob.SetPropertiesAsync();

                            return result;
                        }
                    }
                }
            }

            throw new Exception("You did it wrong. Find help. Or don't.");
        }

        private void CropImage(Image<Rgba32> image, int? width, int? height)
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
                image.Mutate(x => x.Resize(0, newHeight));
            }
            else
            {
                image.Mutate(x => x.Resize(newWidth, 0));

            }

            image.Mutate(x => x.Crop(newWidth, newHeight));

        }

        private void ResizeImage(Image<Rgba32> image, int? width, int? height)
        {
            var newWidth = width ?? image.Width;
            var newHeight = height ?? image.Height;

            // Guard against large width from the query params
            if (newWidth < 1 || newWidth > image.Width)
                newWidth = image.Width;
            
            // Guard against large height from the query params
            if (newHeight < 1 || newHeight > image.Height)
                newHeight = image.Height;

            image.Mutate(x => x.Crop(newWidth, newHeight));
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
