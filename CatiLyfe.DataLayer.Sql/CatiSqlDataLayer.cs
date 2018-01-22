namespace CatiLyfe.DataLayer.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    using CatiLyfe.DataLayer.Models;
    using CatiLyfe.Common.Exceptions;

    using Microsoft.SqlServer.Server;
    using System.Data;

    /// <summary>
    /// The data layer for cati lyfe.
    /// </summary>
    internal sealed class CatiSqlDataLayer : SqlDataLayerBase, ICatiDataLayer
    {
        public CatiSqlDataLayer(string connectionString) : base(connectionString)
        {
        }

        public async Task<IEnumerable<PostMeta>> GetPostMetadata(int? top, int? skip, DateTime? startdate, DateTime? enddate, bool includeUnpublished, bool includeDeleted, IEnumerable<string> tags)
        {
            var results =  await this.ExecuteReader(
                "cati.getpostmetadata",
                parmeters =>
                    {
                        parmeters.AddWithValue("top", top);
                        parmeters.AddWithValue("skip", skip);
                        parmeters.AddWithValue("startdate", startdate);
                        parmeters.AddWithValue("enddate", enddate);
                        parmeters.AddWithValue("includeUnpublished", includeUnpublished);
                        parmeters.AddWithValue("includeDeleted", includeDeleted);
                        var tagslist = parmeters.AddWithValue("tags", CatiSqlDataLayer.GetPostTagRecords(tags));
                        tagslist.SqlDbType = SqlDbType.Structured;
                        tagslist.TypeName = "cati.tagslist";
                    },
                SqlParsers.ParsePostMeta,
                SqlParsers.ParsePostTag,
                SqlParsers.ParsePostAuditMapping);

            var tagsLookup = results.Item2.ToLookup(t => t.PostId);
            var historyLookup = results.Item3.ToLookup(a => a.PostId);

            // Get the tag mapping.
            foreach (var metadata in results.Item1)
            {
                if (tagsLookup.Contains(metadata.Id))
                {
                    metadata.Tags = tagsLookup[metadata.Id].Select(t => t.Tag);
                }

                if (historyLookup.Contains(metadata.Id))
                {
                    metadata.History = historyLookup[metadata.Id].Select(a => a.ToPostAuditHistory());
                }
            }

            return results.Item1;
        }

        public async Task<Post> GetPost(int id, bool includeUnpublished, bool includeDeleted)
        {
            var post = (await this.GetPostInternal(
                        id: id,
                        slug: null,
                        top: null,
                        skip: null,
                        startdate: null,
                        enddate: null,
                        includeUnpublished: includeUnpublished,
                        includeDeleted: includeDeleted,
                        tags: null)).FirstOrDefault();

            if (null == post)
            {
                throw new ItemNotFoundException($"The post with the id '{id}' was not found.");
            }

            return post;
        }

        public async Task<Post> GetPost(string slug, bool includeUnpublished, bool includeDeleted)
        {
            var post = (await this.GetPostInternal(
                        id: null,
                        slug: slug,
                        top: null,
                        skip: null,
                        startdate: null,
                        enddate: null,
                        includeUnpublished: includeUnpublished,
                        includeDeleted: includeDeleted,
                        tags: null)).FirstOrDefault();

            if (null == post)
            {
                throw new ItemNotFoundException($"The post with the slug '{slug}' was not found.");
            }

            return post;
        }

        public async Task<IEnumerable<Post>> GetPost(int? top, int? skip, DateTime? startdate, DateTime? enddate, bool includeUnpublished, bool includeDeleted, IEnumerable<string> tags)
        {
            return (await this.GetPostInternal(
                        id: null,
                        slug: null,
                        top: top,
                        skip: skip,
                        startdate: startdate,
                        enddate: enddate,
                        includeUnpublished: includeUnpublished,
                        includeDeleted: includeDeleted,
                        tags: tags)).ToImmutableList();
        }

        private async Task<IEnumerable<Post>> GetPostInternal(
            int? id,
            string slug,
            int? top,
            int? skip,
            DateTime? startdate,
            DateTime? enddate,
            bool includeUnpublished,
            bool includeDeleted,
            IEnumerable<string> tags)
        {
            var results = await this.ExecuteReader(
                              "cati.getposts",
                              parmeters =>
                                  {
                                      parmeters.AddWithValue("id", id);
                                      parmeters.AddWithValue("slug", slug);
                                      parmeters.AddWithValue("top", top);
                                      parmeters.AddWithValue("skip", skip);
                                      parmeters.AddWithValue("startdate", startdate);
                                      parmeters.AddWithValue("enddate", enddate);
                                      parmeters.AddWithValue("includeUnpublished", includeUnpublished);
                                      parmeters.AddWithValue("includeDeleted", includeDeleted);
                                      var tagslist = parmeters.AddWithValue("tags", CatiSqlDataLayer.GetPostTagRecords(tags));
                                      tagslist.SqlDbType = SqlDbType.Structured;
                                      tagslist.TypeName = "cati.tagslist";
                                  },
                              SqlParsers.ParsePostMeta,
                              SqlParsers.ParsePostContent,
                              SqlParsers.ParsePostTag,
                              SqlParsers.ParsePostAuditMapping);

            var postContentlookup = results.Item2.ToLookup(c => c.PostId);
            var tagsLookup = results.Item3.ToLookup(t => t.PostId);
            var historyLookup = results.Item4.ToLookup(a => a.PostId);

            // Get the tag mapping.
            foreach (var metadata in results.Item1)
            {
                if (tagsLookup.Contains(metadata.Id))
                {
                    metadata.Tags = tagsLookup[metadata.Id].Select(t => t.Tag);
                }

                if (historyLookup.Contains(metadata.Id))
                {
                    metadata.History = historyLookup[metadata.Id].Select(a => a.ToPostAuditHistory());
                }
            }

            return results.Item1.Select(meta => new Post(meta, postContentlookup.First(m => m.Key == meta.Id)));
        }

        /// <summary>
        /// Gets all of the tags.
        /// </summary>
        /// <returns>The tags.</returns>
        public async Task<IEnumerable<PostTag>> GetTags()
        {
            var result = await this.ExecuteReader(
                "cati.gettags",
                parmeters =>
                {
                },
                SqlParsers.ParseTag);

            return result;
        }

        /// <summary>
        /// Set a post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <param name="userAccessDetails">The user access details.</param>
        /// <returns>An async task.</returns>
        public async Task<Post> SetPost(Post post, UserAccessDetails userAccessDetails)
        {
            var results = await this.ExecuteReader(
                              "cati.setpost",
                              parmeters =>
                                  {
                                      parmeters.AddWithValue("id", post.MetaData.Id <= 0 ? null : (int?)post.MetaData.Id);
                                      parmeters.AddWithValue("slug", post.MetaData.Slug);
                                      parmeters.AddWithValue("title", post.MetaData.Title);
                                      parmeters.AddWithValue("description", post.MetaData.Description);
                                      parmeters.AddWithValue("userid", userAccessDetails.UserId);
                                      parmeters.AddWithValue("goeslive", post.MetaData.GoesLive);
                                      parmeters.AddWithValue("ispublished", post.MetaData.IsPublished);
                                      parmeters.AddWithValue("isreserved", post.MetaData.IsReserved);
                                      parmeters.AddWithValue("revision", post.MetaData.Revision);
                                      parmeters.AddWithValue("publisheduser", post.MetaData.PublishedUser);

                                      var contentList = parmeters.AddWithValue(
                                          "content",
                                          CatiSqlDataLayer.GetPostContentRecord(post.PostContent));
                                      contentList.SqlDbType = SqlDbType.Structured;
                                      contentList.TypeName = "cati.postcontentlist";
                                      var tagslist = parmeters.AddWithValue(
                                          "tags",
                                          CatiSqlDataLayer.GetPostTagRecords(post.MetaData.Tags));
                                      tagslist.SqlDbType = SqlDbType.Structured;
                                      tagslist.TypeName = "cati.tagslist";
                                  },
                              SqlParsers.ParsePostMeta,
                              SqlParsers.ParsePostContent,
                              SqlParsers.ParsePostTag,
                              SqlParsers.ParsePostAuditMapping);

            var metadata = results.Item1.First();
            var tags = results.Item3;
            metadata.Tags = tags.Select(t => t.Tag);

            var history = results.Item4;
            metadata.History = history.Select(h => h.ToPostAuditHistory());

            return new Post(results.Item1.First(), results.Item2);
        }

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="id">The post id.</param>
        /// <param name="userAccessDetails">The user details</param>
        /// <returns>An async task</returns>
        public Task DeletePost(int id, UserAccessDetails userAccessDetails)
        {
            return this.ExecuteNonQuery(
                "cati.deletepost",
                parrameters =>
                {
                    parrameters.AddWithValue("id", id);
                    parrameters.AddWithValue("userid", userAccessDetails.UserId);
                });
        }

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="slug">The post slug.</param>
        /// <param name="userAccessDetails">The user details</param>
        /// <returns>An async task</returns>
        public Task DeletePost(string slug, UserAccessDetails userAccessDetails)
        {
            return this.ExecuteNonQuery(
                "cati.deletepost",
                parrameters =>
                {
                    parrameters.AddWithValue("slug", slug);
                    parrameters.AddWithValue("userid", userAccessDetails.UserId);
                });
        }

        /// <summary>
        /// Gets records for post tags.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns>The enumerable.</returns>
        private static IEnumerable<SqlDataRecord> GetPostTagRecords(IEnumerable<string> tags)
        {
            return tags.ToDataTable(
                () => new[] { new SqlMetaData("tag", SqlDbType.NVarChar, 64) },
                (record, tag) =>
                    {
                        record.SetValue(0, tag);
                    });
        }

        /// <summary>
        /// Gets records for post content.
        /// </summary>
        /// <param name="content">The content of the post.</param>
        /// <returns>The post content.</returns>
        private static IEnumerable<SqlDataRecord> GetPostContentRecord(IEnumerable<PostContent> content)
        {
            return content.ToDataTable(
                () => new[]
                          {
                              new SqlMetaData("id", SqlDbType.Int),
                              new SqlMetaData("type", SqlDbType.NVarChar, 64),
                              new SqlMetaData("content", SqlDbType.NVarChar, 4000),
                          },
                (record, tag) =>
                    {
                        record.SetValue(0, tag.Index);
                        record.SetValue(1, tag.ContentType);
                        record.SetValue(2, tag.Content);
                    });
        }
    }
}
