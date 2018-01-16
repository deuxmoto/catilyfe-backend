namespace CatiLyfe.Backend.Web.Models
{
    using System;
    using System.Collections.Generic;

    using CatiLyfe.DataLayer.Models;

    /// <summary>
    /// The post meta model.
    /// </summary>
    public class PostMetaModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostMetaModel"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        public PostMetaModel(int id, string slug, string title, string description, DateTime whenPublished, string author, IEnumerable<string> tags)
        {
            this.Id = id;
            this.Slug = slug;
            this.Title = title;
            this.Description = description;
            this.WhenPublished = whenPublished;
            this.Author = author;
            this.Tags = tags;
        }

        public int Id { get; }
        public string Slug { get; }

        public string Title { get; }

        public DateTimeOffset WhenPublished { get; }

        public string Description { get; }

        public string Author { get; }

        public IEnumerable<string> Tags { get; }
    }
}