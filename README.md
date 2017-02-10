[![Build status](https://ci.appveyor.com/api/projects/status/nc3782ai2n0u7wtp?svg=true)](https://ci.appveyor.com/project/Q42/q42-imageresizert)


# Q42.ImageResizert
Image resizing for dotnet core mvc projects using ImageMagick

Include this package in any dot net core mvc file and `/image/{imageId}?width=200&height=300&cover=true` will handle imageresizing.

### Configure
```cs
services.Configure<ImageResizerSettings>(Configuration.GetSection("ImageResizerSettings"));
```

```json
"ImageResizerSettings": {
    "AzureConnectionString": "DefaultEndpointsProtocol=https;AccountName=<yourname>;AccountKey=<yourkey>",
    "AssetContainerName": "<yourname>",    
    "ImageCdn": "<http://optional:url>",
    "CompressionQuality": "<1-100>"
  }
  ```
