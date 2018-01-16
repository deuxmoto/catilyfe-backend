namespace CatiLyfe.Backend.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    using CatiLyfe.DataLayer.Models;

    /// <summary>
    /// The post.
    /// </summary>
    public class PostModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostModel"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="html">The html.</param>
        internal PostModel(PostMetaModel metadata, string html)
        {
            this.Metadata = metadata;
            this.RawHtmlThenIGuess = html;
        }

        /// <summary>
        /// The metadata for the post.
        /// </summary>
        [Required]
        public PostMetaModel Metadata { get; }

        /// <summary>
        /// The content for the post.
        /// </summary>
        [Required]
        public string RawHtmlThenIGuess { get; }
    }
}
