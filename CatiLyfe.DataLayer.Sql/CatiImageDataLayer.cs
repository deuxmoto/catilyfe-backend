using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CatiLyfe.DataLayer.Models.Images;
using System.Data.SqlClient;
using System.Linq;
using CatiLyfe.Common.Utilities;

namespace CatiLyfe.DataLayer.Sql
{
    internal class CatiImageDataLayer : SqlDataLayerBase, ICatiImageDataLayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatiImageDataLayer"/> class.
        /// </summary>
        /// <param name="connectionString">Th connection string.</param>
        public CatiImageDataLayer(string connectionString) : base(connectionString)
        {

        }

        /// <summary>
        /// Get matching images.
        /// </summary>
        /// <param name="id">The image id.</param>
        /// <param name="slug">The image slug.</param>
        /// <param name="top">Number to get.</param>
        /// <param name="skip">Number to skip.</param>
        /// <returns>The images.</returns>
        public async Task<IReadOnlyCollection<Image>> GetImage(int? id, string slug = null, int top = 100, int skip = 0)
        {
            var result = await this.ExecuteReader("img.getimage", parameters =>
            {
                parameters.AddWithValue("id", id);
                parameters.AddWithValue("slug", slug);
                parameters.AddWithValue("top", top);
                parameters.AddWithValue("skip", skip);
            },
            CatiImageDataLayer.ReadLink,
            CatiImageDataLayer.ReadImage);

            var linklookup = result.Item1.ToLookup(lnk => lnk.ImageId);
            foreach(var img in result.Item2)
            {
                if(linklookup.Contains(img.Id))
                {
                    foreach(var link in linklookup[img.Id])

                    img.Links.Add(link);
                }
            }

            return result.Item2.AsReadonly();
        }

        public async Task<Image> SetImageDetails(Image image)
        {
            var result = await this.ExecuteReader("img.setimagedetails", parameters =>
            {
                parameters.AddWithValue("id", image.Id);
                parameters.AddWithValue("slug", image.Slug);
                parameters.AddWithValue("description", image.Description);
            },
            CatiImageDataLayer.ReadLink,
            CatiImageDataLayer.ReadImage);

            var linklookup = result.Item1.ToLookup(lnk => lnk.ImageId);
            foreach (var img in result.Item2)
            {
                if (linklookup.Contains(img.Id))
                {
                    foreach (var link in linklookup[img.Id])

                        img.Links.Add(link);
                }
            }

            return result.Item2.Single();
        }

        /// <summary>
        /// Sets an image link.
        /// </summary>
        /// <param name="link">The link</param>
        /// <returns>The image.</returns>
        public async Task<Image> SetImageLinks(ImageLink link)
        {
            var result = await this.ExecuteReader("img.setimagelink", parameters =>
            {
                parameters.AddWithValue("imageid", link.ImageId);
                parameters.AddWithValue("linkid", link.LinkId);
                parameters.AddWithValue("filetype", link.Format);
                parameters.AddWithValue("width", link.Width);
                parameters.AddWithValue("height", link.Height);
                parameters.AddWithValue("adapter", link.Adapter);
                parameters.AddWithValue("metadata", link.Metadata);
            },
            CatiImageDataLayer.ReadLink,
            CatiImageDataLayer.ReadImage);

            var linklookup = result.Item1.ToLookup(lnk => lnk.ImageId);
            foreach (var img in result.Item2)
            {
                if (linklookup.Contains(img.Id))
                {
                    foreach (var lnData in linklookup[img.Id])
                    {
                        img.Links.Add(lnData);
                    }
                }
            }

            return result.Item2.Single();
        }

        /// <summary>
        /// Read a link from SQL.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The iamge link</returns>
        private static ImageLink ReadLink(SqlDataReader reader)
        {
            var adapterString = (string)reader["adapter"];
            var adapter = Enum.Parse<ImageAdapter>(adapterString);

            return new ImageLink((int)reader["image"], (int)reader["id"], (int)reader["width"], (int)reader["height"], (string)reader["fileformat"], adapter, (string)reader["metadata"]);
        }

        /// <summary>
        /// Read an image from sql.
        /// </summary>
        /// <param name="reader">The sql data reader.</param>
        /// <returns>The image.</returns>
        private static Image ReadImage(SqlDataReader reader)
        {
            return new Image((int)reader["id"], (string)reader["slug"], (string)reader["description"], (DateTime)reader["whencreated"], Enumerable.Empty<ImageLink>());
        }
    }
}
