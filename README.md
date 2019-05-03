[![Build status](https://ci.appveyor.com/api/projects/status/nc3782ai2n0u7wtp?svg=true)](https://ci.appveyor.com/project/Q42/q42-imageresizert)

# Q42.ImageResizert
Image resizing for dotnet core mvc projects using ImageSharp and Azure storage.

Include this package in any dot net core mvc file and `/image/{imageId}?w=200&h=300&cover=true&quality=90` will handle imageresizing.

## Q42.ImageResizertService

You can also use the ImageResizertService directly 
```cs
var service = new ImageResizertService(settings);
```

### Configure
```cs
services.Configure<ImageResizertSettings>(Configuration.GetSection("ImageResizerSettings"));
```

```json
"ImageResizerSettings": {
    "AzureConnectionString": "DefaultEndpointsProtocol=https;AccountName=<yourname>;AccountKey=<yourkey>",
    "AssetContainerName": "<yourname>",    
    "CompressionQuality": "<1-100>",
    
    // optional settings
    "CacheContainerName":  "<mycachecontanier>",
    "CacheFolderName": "<mycachefolder>", // defaults to imagecache
    "BaseUrl": "<your application url>" // when configured, only absolute urls will be returned
    "ImageCdn": "<your cdn url>" // when configured, only absolute urls will be returned. Overrides BaseUrl
  }
```

### Caching
By default, all images that are created and stored in `<CacheFolderName>/{id}-{width}-{height}-{cover}-{quality}`. Currently you can only clear these by deleting the imagecache folder.

### Razor view helper
To use the UrlHelper shorthands
```cs
<img src="@UrlHelper.GetBaseUrlForImage(item.ImageId)" />
```

add this line to `_ViewImports.csthml`
```cs
@inject Q42.ImageResizer.UrlHelper UrlHelper
```

