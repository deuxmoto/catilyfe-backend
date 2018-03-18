using CatiLyfe.Backend.ImageServices;
using CatiLyfe.Backend.Web.Models.Images;
using CatiLyfe.Common.Exceptions;
using CatiLyfe.DataLayer;
using CatiLyfe.DataLayer.Models.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CatiLyfe.Backend.Web.Core.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly ICatiImageDataLayer imageData;
        private readonly IImageUploader uploader;

        public ImageController(ICatiImageDataLayer imageData, IImageUploader uploader)
        {
            this.imageData = imageData;
            this.uploader = uploader;
        }

        [HttpGet]
        public async Task<IEnumerable<ImageModel>> GetImages(int? id = null, string slug = null, int top = 100, int skip = 0)
        {
            var dbImages = await this.imageData.GetImage(id: id, slug: slug, top: top, skip: skip);

            return await Task.WhenAll(dbImages.Select(i => this.GetImage(i)));
        }

        [HttpPost]
        public async Task<ImageModel> CreateImage(IFormFile file)
        {
            var headers = this.Request.Headers;
            int.TryParse(headers["cati-image-id"], out var id);
            var slug = headers["cati-image-slug"];
            var description = headers["cati-image-description"];

            if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(description))
            {
                throw new ModelValidationException("Neither slug or description can be empty.");
            }

            var img = new Image(id == 0 ? (int?)null : id, slug, description, Enumerable.Empty<ImageLink>());

            using (var reader = file.OpenReadStream())
            {
                var result = await this.uploader.SetImage(img, reader);

                return await this.GetImage(result);
            }
        }

        private async Task<ImageModel> GetImage(Image image)
        {
            var links = await this.GetLinks(image.Links);
            return new ImageModel(image.Id.Value, image.Description, image.Slug, links);
        }

        /// <summary>
        /// Gets all of the links from the db model.
        /// </summary>
        /// <param name="links">The links.</param>
        /// <returns>The link models.</returns>
        private async Task<IReadOnlyCollection<ImageLinkModel>> GetLinks(IEnumerable<ImageLink> links)
        {
            return await Task.WhenAll(links.Select(async l =>
            {
                var url = await this.uploader.GetUrl(l);
                var lnk = new ImageLinkModel(l.LinkId.Value, l.Width, l.Height, url);
                return lnk;
            }
                ));
        }
    }
}