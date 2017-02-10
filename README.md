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
