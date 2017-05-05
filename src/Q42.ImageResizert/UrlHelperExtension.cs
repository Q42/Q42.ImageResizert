using Microsoft.Extensions.Options;
using System;

namespace Q42.ImageResizert
{
    public class ImageResizert
    {
        private readonly ImageResizertSettings storageSettings;

        public ImageResizert(IOptions<ImageResizertSettings> storageSettings)
        {
            this.storageSettings = storageSettings.Value;
        }

        /// <summary>
        /// Resize image and cover
        /// </summary>
        /// <param name="id"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public string GetUrlForCoverImage(string id, int width, int height)
        {
            return GetUrlForImage(id, width, height, true);
        }

        /// <summary>
        /// Distinguish between CDN or image resize url
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetBaseUrlForImage(string id)
        {
            string url = "/image?id=" + id;

            if (!string.IsNullOrWhiteSpace(storageSettings.ImageCdn))
                url = storageSettings.ImageCdn + url;

            return url;
        }

        public string GetUrlForImage(string id, int? width, int? height = null, bool? cover = null, int? quality = null)
        {
            string url = GetBaseUrlForImage(id);

            if (width.HasValue)
                url += "&w=" + width.Value;

            if (height.HasValue)
                url += "&h=" + height.Value;

            if (cover.HasValue)
                url += "&cover=" + cover.Value;

            if (quality.HasValue)
                url += "&quality=" + quality.Value;

            return url;
        }
    }
}
