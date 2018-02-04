using CatiLyfe.DataLayer.Models.Images;
using System.IO;
using System.Threading.Tasks;

namespace CatiLyfe.Backend.ImageServices
{
    public interface IImageUploader
    {
        /// <summary>
        /// Creates or updates an image.
        /// </summary>
        /// <param name="details">The image.</param>
        /// <param name="image">The stream.</param>
        /// <returns>The image result.</returns>
        Task<Image> SetImage(Image details, Stream image);


        /// <summary>
        /// Gets the url redirect for a link.
        /// </summary>
        /// <param name="link">The links.</param>
        /// <returns>The url.</returns>
        Task<string> GetUrl(ImageLink link);
    }
}
