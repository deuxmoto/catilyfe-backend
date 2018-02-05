using CatiLyfe.DataLayer.Models.Images;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatiLyfe.DataLayer
{
    /// <summary>
    /// The cati image data layer.
    /// </summary>
    public interface ICatiImageDataLayer
    {
        /// <summary>
        /// Get matching images.
        /// </summary>
        /// <param name="slug">The image slug.</param>
        /// <param name="top">Number to get.</param>
        /// <param name="skip">Number to skip.</param>
        /// <returns>The images.</returns>
        Task<IReadOnlyCollection<Image>> GetImage(string slug = null, int top = 100, int skip = 0);

        /// <summary>
        /// Set an image details.
        /// </summary>
        /// <param name="img">The details.</param>
        /// <returns>The image.</returns>
        Task<Image> SetImageDetails(Image img);

        /// <summary>
        /// Sets an image link.
        /// </summary>
        /// <param name="link">The link</param>
        /// <returns>The image.</returns>
        Task<Image> SetImageLinks(ImageLink link);
    }
}
