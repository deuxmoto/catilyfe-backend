namespace CatiLyfe.Backend.Web.Models.Images
{
    public class ImageLinkModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLinkModel"/> class.
        /// </summary>
        /// <param name="id">The link id.</param>
        /// <param name="url">The url.</param>
        public ImageLinkModel(int id, int width, int height, string url)
        {
            this.Id = id;
            this.Width = width;
            this.Height = height;
            this.Url = url;
        }

        /// <summary>
        /// The id of the image.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The image width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The image height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The url for the image.
        /// </summary>
        public string Url { get; private set; }
    }
}
