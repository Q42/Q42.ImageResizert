namespace Q42.ImageResizert
{
  public class ImageResizertSettings
  {
    /// <summary>
    /// Connection string for Azure storage account
    /// </summary>
    public string AzureConnectionString { get; set; }

    public string AssetContainerName { get; set; }

    /// <summary>
    /// If no Cdn is configured the imageUrl defaults to /image
    /// </summary>
    public string ImageCdn { get; set; }

    /// <summary>
    /// Default 75
    /// </summary>
    public int CompressionQuality { get; set; }
  }
}
