namespace CatiLyfe.DataLayer.Models.Images
{
    /// <summary>
    /// The image link class.
    /// </summary>
    public class ImageLink
    {
        /// <summary>
        /// Initializes an instance of the <see cref="ImageLink"/> class.
        /// </summary>
        /// <param name="imageid">The image id.</param>
        /// <param name="linkId">The link id.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="format">The format of the image</param>
        /// <param name="adapter">The adapter of the image.</param>
        /// <param name="metadata">The image matadata.</param>
        public ImageLink(int? imageid, int? linkId, int width, int height, string format, ImageAdapter adapter, string metadata)
        {
            this.ImageId = imageid;
            this.LinkId = linkId;
            this.Width = width;
            this.Height = height;
            this.Format = format;
            this.Adapter = adapter;
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the image id.
        /// </summary>
        public int? ImageId { get; }

        /// <summary>
        /// Gets the link id.
        /// </summary>
        public int? LinkId { get; }

        /// <summary>
        /// Gets the image width.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the image height
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the image format
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Gets the image adapter type
        /// </summary>
        public ImageAdapter Adapter { get; }

        /// <summary>
        /// Gets the image metadata
        /// </summary>
        public string Metadata { get; }
    }
}
