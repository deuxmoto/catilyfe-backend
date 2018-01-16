using CatiLyfe.Backend.Web.Models.User;
using CatiLyfe.Common.Security;
using CatiLyfe.DataLayer;
using CatiLyfe.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatiLyfe.Backend.Web.Models
{
    internal sealed class PostTranslator : IPostTranslator
    {
        /// <summary>
        /// The authorization data layer;
        /// </summary>
        private readonly ICatiAuthDataLayer authDataLayer;

        /// <summary>
        /// The content transformer for the post.
        /// </summary>
        private readonly IContentTransformer contentTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostTranslator"/> class.
        /// </summary>
        /// <param name="authData"></param>
        public PostTranslator(ICatiAuthDataLayer authData, IContentTransformer contentTransformer)
        {
            this.authDataLayer = authData;
            this.contentTransformer = contentTransformer;
        }

        /// <summary>
        /// Gets all of the post models;
        /// </summary>
        /// <param name="posts">The posts to return.</param>
        /// <returns>The post models.</returns>
        public async Task<IReadOnlyCollection<PostModel>> GetPostModels(ICollection<Post> posts)
        {
            var metas = await this.GetMetaDatas(posts.Select(p => p.MetaData).ToList());
            var metaDict = metas.ToDictionary(m => m.Id);

            var result = new List<PostModel>();
            foreach(var post in posts)
            {
                var model = new PostModel(metaDict[post.MetaData.Id], this.contentTransformer.TransformMarkdown(string.Join(Environment.NewLine, post.PostContent.Select(c => c.Content))));
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// Translates the post meta object to the public contract.
        /// </summary>
        /// <param name="metas"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<PostMetaModel>> GetMetaDatas(ICollection<PostMeta> metas)
        {
            var requiredUsers = metas.Select(m => m.PublishedUser).Concat(metas.SelectMany(m => m.History.Select(h => h.UserId))).Distinct();

            var users = await this.authDataLayer.GetUser(ids: requiredUsers, emails: null, names: null, token: null);
            var userslookup = users.ToLookup(u => u.Id);

            var results = new List<PostMetaModel>();
            foreach(var user in metas)
            {
                var model = new PostMetaModel(user.Id, user.Slug, user.Title, user.Description, user.GoesLive.DateTime, userslookup[user.PublishedUser].Single().Name, user.Tags);

                results.Add(model);
            }

            return results;
        }

        /// <summary>
        /// Translates a user model to a user.
        /// </summary>
        /// <param name="model">The user model.</param>
        /// <returns>The user.</returns>
        public Task<DataLayer.Models.User> TranslateUser(UserModel model)
        {
            byte[] password = null;
            byte[] salt = null;
            if(null != model.Password)
            {
                salt = PasswordGenerator.GenerateRandom(64);
                password = PasswordGenerator.HashPassword(salt, model.Password);
            }

            var result = new DataLayer.Models.User(model.Id, model.Name, model.Email, salt, password, model.Roles.ToHashSet());
            return Task.FromResult(result);
        }
    }
}
