namespace Q42.ImageResizert
{
    public class ImageResizertSettings
    {
        /// <summary>
        /// Connection string for Azure storage account.
        /// </summary>
        public string AzureConnectionString { get; set; }

        /// <summary>
        /// Name of the container where the assets are located.
        /// </summary>
        public string AssetContainerName { get; set; }

        /// <summary>
        /// Name of the container where the images will be cached. If null, will use the AssetContainerName.
        /// </summary>
        public string CacheContainerName { get; set; }

        /// <summary>
        /// Name of the folder where the images will be cached. Defaults to "imagecache". Can be null or empty.
        /// </summary>
        public string CacheContainerFolder { get; set; }

        /// <summary>
        /// Default quality used when no quality has been given
        /// </summary>
        public int CompressionQuality { get; set; }

        /// <summary>
        /// If no Cdn is configured the imageUrl defaults to /image
        /// </summary>
        public string ImageCdn { get; set; }

        /// <summary>
        /// Base url of your application, including the scheme e.g: https://myhost.com
        /// Used in absolute url paths
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
