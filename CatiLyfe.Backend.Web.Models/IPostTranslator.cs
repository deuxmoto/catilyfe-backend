using CatiLyfe.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CatiLyfe.Backend.Web.Models
{
    public interface IPostTranslator
    {
        /// <summary>
        /// Gets all of the post models;
        /// </summary>
        /// <param name="posts">The posts to return.</param>
        /// <returns>The post models.</returns>
        Task<IReadOnlyCollection<PostModel>> GetPostModels(ICollection<Post> posts);

        /// <summary>
        /// Translates the post meta object to the public contract.
        /// </summary>
        /// <param name="metas"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<PostMetaModel>> GetMetaDatas(ICollection<PostMeta> metas);
    }
}
