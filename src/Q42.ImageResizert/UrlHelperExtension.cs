using Microsoft.Extensions.Options;
using System;

namespace Q42.ImageResizert
{
  public class ImageResizert
  {
    private readonly ImageResizertSettings _storageSettings;

    public ImageResizert(IOptions<ImageResizertSettings> storageSettings)
    {
      _storageSettings = storageSettings.Value;
    }

    /// <summary>
    /// Resize image and cover
    /// </summary>
    /// <param name="id"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public string GetUrlForCoverImage(Guid id, int width, int height)
    {
      return GetUrlForImage(id.ToString().ToUpper(), width, height, true);
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
    /// Resize image while maintaining aspect ratio
    /// </summary>
    /// <param name="id"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public string GetUrlForScaledImage(Guid id, int width = 0, int height = 0)
    {
      return GetUrlForImage(id.ToString().ToUpper(), width, height, false);
    }

    /// <summary>
    /// Distinguish between CDN or image resize url
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetBaseUrlForImage(Guid id)
    {
      return GetBaseUrlForImage(id.ToString().ToUpper());
    }

    /// <summary>
    /// Distinguish between CDN or image resize url
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetBaseUrlForImage(string id)
    {
      string url = "/image?id=" + id;

      if (!string.IsNullOrWhiteSpace(_storageSettings.ImageCdn))
        url = _storageSettings.ImageCdn + url;

      return url;
    }

    private string GetUrlForImage(string id, int width = 0, int height = 0, bool cover = false)
    {
      string url = GetBaseUrlForImage(id);

      if (width > 0)
        url += "&width=" + width;

      if (height > 0)
        url += "&height=" + height;

      if (cover)
        url += "&cover=" + cover;   

      return url;
    }
  }
}
