﻿namespace CatiLyfe.Backend.Web.Models
{
    public interface IContentTransformer
    {
        /// <summary>
        /// The transform markdown.
        /// </summary>
        /// <param name="markdown">The markdown.</param>
        /// <returns>The <see cref="string"/>.</returns>
        string TransformMarkdown(string markdown);
    }
}
