using System;
using System.IO;
using System.Threading.Tasks;
using CatiLyfe.Backend.ImageServices.Models;
using CatiLyfe.Common.Utilities;
using CatiLyfe.DataLayer;
using CatiLyfe.DataLayer.Models.Images;
using Microsoft.WindowsAzure.Storage;
using SkiaSharp;

namespace CatiLyfe.Backend.ImageServices
{
    internal class AzureStorageImageUploader  : IImageUploader
    {
        private readonly ICatiImageDataLayer imageData;

        private readonly CloudStorageAccount account;

        public AzureStorageImageUploader(ICatiImageDataLayer imageData, string connectionString)
        {
            this.imageData = imageData;
            this.account = CloudStorageAccount.Parse(connectionString);
        }

        public Task<string> GetUrl(ImageLink link)
        {
            if(link.Adapter != ImageAdapter.AzureFile)
            {
                throw new NotImplementedException("Only azure storage is supproted");
            }

            var meta = AzureAdapterMetadata.Parse(link.Metadata);
            return Task.FromResult(meta.Url);
        }

        public async Task<Image> SetImage(Image details, Stream image)
        {
            var dbImage = await this.imageData.SetImageDetails(details);
            var fileClient = this.account.CreateCloudBlobClient();
            var container = fileClient.GetContainerReference($"cati-{dbImage.Id}");
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions
            {
                PublicAccess = Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Blob
            });

            int width, height;
            string name;
            string format;
            string url;

            using (var input = new SKManagedStream(image))
            using (var codec = SKCodec.Create(input))
            {
                var newHeight = ComplexMath.RoundDownP2(codec.Info.Height);
                var ratio = (float)newHeight / (float)codec.Info.Height;
                var supportedScale = codec.GetScaledDimensions(ratio);

                width = supportedScale.Width;
                height = supportedScale.Height;
                format = codec.EncodedFormat.ToString();
                name = $"{dbImage.Slug}_{width}_{height}";

                var blobRef = container.GetBlockBlobReference(name);
                url = blobRef.StorageUri.PrimaryUri.ToString();

                using (var resultBitmap = SKBitmap.Decode(codec))
                using (var resizedBitmap = resultBitmap.Resize(new SKImageInfo(supportedScale.Width, supportedScale.Height), SKBitmapResizeMethod.Lanczos3))
                using (var resultImage = SKImage.FromBitmap(resizedBitmap))
                using (var imageStream = resultImage.Encode(codec.EncodedFormat, 100).AsStream())
                {
                    using (var outputSTream = await blobRef.OpenWriteAsync())
                    {
                        await imageStream.CopyToAsync(outputSTream);
                    }
                }
            }

            var data = new AzureAdapterMetadata(this.account.BlobEndpoint.ToString(), container.Name, name, url);

            dbImage = await this.imageData.SetImageLinks(new ImageLink(dbImage.Id, null, width, height, format, ImageAdapter.AzureFile, data.ToString()));

            return dbImage;
        }
    }
}
