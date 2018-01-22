using System.Collections.Generic;

namespace CatiLyfe.DataLayer.Models.Images
{
    public class Image
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="slug">The slug.</param>
        /// <param name="description">The description</param>
        /// <param name="links">The list of links.</param>
        public Image(int? id, string slug, string description, IEnumerable<ImageLink> links)
        {
            this.Id = id;
            this.Slug = slug;
            this.Description = description;
            this.Links = new List<ImageLink>(links);
        }

        /// <summary>
        /// Gets the image id.
        /// </summary>
        public int? Id { get;}

        /// <summary>
        /// Gets the image slug.
        /// </summary>
        public string Slug { get; }

        /// <summary>
        /// Gets the image description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the list of image links
        /// </summary>
        public IList<ImageLink> Links { get; }
    }
}
