using System;
using System.Collections.Generic;
using System.Linq;

namespace CatiLyfe.Backend.Web.Models.Images
{
    public class ImageModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageModel"/> class.
        /// </summary>
        /// <param name="id">The image id.</param>
        /// <param name="description">The image description.</param>
        /// <param name="slug">The image slug.</param>
        /// <param name="whenCreated">When the image was created.</param>
        /// <param name="links">The image links.</param>
        public ImageModel(int id, string description, string slug, DateTime whenCreated, IEnumerable<ImageLinkModel> links)
        {
            this.Id = id;
            this.Description = description;
            this.Slug = slug;
            this.WhenCreated = whenCreated;
            this.Links = links.ToArray();
        }

        /// <summary>
        /// The image id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The image description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the slug.
        /// </summary>
        public string Slug { get; private set; }

        /// <summary>
        /// Gets when the image was created.
        /// </summary>
        public DateTime WhenCreated { get; private set; }

        /// <summary>
        /// All available links.
        /// </summary>
        public ImageLinkModel[] Links { get; private set; }
    }
}
