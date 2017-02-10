using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;
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

    public string GetUrlForCoverImage(Guid id, int width, int height, string containerName = null)
    {
      return GetUrlForImage(id.ToString().ToUpper(), width, height, true, containerName);
    }

    public string GetUrlForCoverImage(string id, int width, int height, string containerName = null)
    {
      return GetUrlForImage(id, width, height, true, containerName);
    }

    public string GetUrlForScaledImage(Guid id, int width = 0, int height = 0, string containerName = null)
    {
      return GetUrlForImage(id.ToString().ToUpper(), width, height, false, containerName);
    }

    public string GetBaseUrlForMicrioImage(Guid id)
    {
      return GetBaseUrlForImage(id.ToString()) + "&containername=" + _storageSettings.MicrioContainerName;
    }

    public string GetBaseUrlForImage(Guid id)
    {
      return GetBaseUrlForImage(id.ToString().ToUpper());
    }

    public string GetBaseUrlForImage(string id)
    {
      string url = "/image?id=" + id;

      if (!string.IsNullOrWhiteSpace(_storageSettings.ImageCdn))
        url = _storageSettings.ImageCdn + url;

      return url;
    }

    private string GetUrlForImage(string id, int width = 0, int height = 0, bool cover = false, string containerName = null)
    {
      string url = GetBaseUrlForImage(id);

      if (width > 0)
        url += "&width=" + width;

      if (height > 0)
        url += "&height=" + height;

      if (cover)
        url += "&cover=" + cover;

      if (!string.IsNullOrWhiteSpace(containerName))
        url += "&containerName=" + containerName;

      return url;
    }
  }
}
