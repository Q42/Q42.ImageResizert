using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Q42.ImageResizert
{
    public class UrlHelper
    {
        private readonly ImageResizertSettings storageSettings;

        public UrlHelper(IOptions<ImageResizertSettings> storageSettings) : this(storageSettings.Value)
        {
        }

        public UrlHelper(ImageResizertSettings storageSettings)
        {
            this.storageSettings = storageSettings;
        }
        
        /// <summary>
        /// Get the base url based on the configuration.
        /// Will return an absolute URL when the ImageCdn or BaseUrl have been set. ImageCDN will have priority over BaseUrl.
        /// Distinguish between CDN or image resize url
        /// </summary>
        /// <param name="id"></param>
        /// <param name="kind">URL type of your application. The CDN URL will always be absolute.</param>
        /// <returns></returns>
        public Uri GetBaseUrlForImage(string id)
        {
            var path = new Uri(string.Format("/image/{0}", id), UriKind.Relative);

            if (!string.IsNullOrEmpty(storageSettings.ImageCdn))
                return new Uri(string.Format("{0}{1}", storageSettings.ImageCdn, path.ToString()), UriKind.Absolute);
            else if (!string.IsNullOrEmpty(storageSettings.BaseUrl))
                return new Uri(string.Format("{0}{1}", storageSettings.BaseUrl, path.ToString()), UriKind.Absolute);
            else
                return path;
        }

        /// <summary>
        /// Get the URL for the given image with the options. Will prefix the CDN url if configured
        /// </summary>
        /// <param name="id">Asset identifier</param>
        /// <param name="kind">URL type of your application. The CDN URL will always be absolute.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="cover"></param>
        /// <param name="quality">0-100</param>
        /// <returns></returns>
        public Uri GetUrlForImage(string id, int? width = null, int? height = null, bool? cover = null, int? quality = null)
        {
            var parameters = new Dictionary<string, string>();

            if (width.HasValue)
                parameters["w"] = width.Value.ToString();

            if (height.HasValue)
                parameters["h"] = height.Value.ToString();
            
            if (cover.HasValue)
                parameters["cover"] = cover.Value.ToString().ToLower();

            if (quality.HasValue)
                parameters["quality"] = quality.Value.ToString();

            var qs = parameters.Any() ? parameters
                .Keys
                .Select(k => string.Format("{0}={1}", k, parameters[k]))
                .Aggregate((current, next) => current + "&" + next) : "";

            var baseUrl = GetBaseUrlForImage(id);
            
            if (string.IsNullOrEmpty(qs))
            {
                return baseUrl;
            }

            return new Uri(string.Format("{0}?{1}", baseUrl, qs), baseUrl.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }
    }
}
