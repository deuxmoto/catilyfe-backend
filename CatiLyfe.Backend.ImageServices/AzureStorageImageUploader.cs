using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CatiLyfe.Backend.ImageServices.Models;
using CatiLyfe.Common.Utilities;
using CatiLyfe.DataLayer;
using CatiLyfe.DataLayer.Models.Images;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SkiaSharp;

namespace CatiLyfe.Backend.ImageServices
{
    internal class AzureStorageImageUploader  : IImageUploader
    {
        private readonly ICatiImageDataLayer imageData;

        private readonly CloudStorageAccount account;

        private readonly int[] imageWidths;

        public AzureStorageImageUploader(ICatiImageDataLayer imageData, string connectionString, int[] imageWidths)
        {
            this.imageData = imageData;
            this.account = CloudStorageAccount.Parse(connectionString);
            this.imageWidths = imageWidths;
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

            using (var input = new SKManagedStream(image))
            using (var codec = SKCodec.Create(input))
            {
                foreach(var width in this.imageWidths)
                {
                    await this.UploadToAzure(dbImage, codec, container, width);
                }
            }

            return (await this.imageData.GetImage(id: dbImage.Id.Value)).Single();
        }

        private async Task UploadToAzure(Image dbImage, SKCodec codec, CloudBlobContainer container, int desiredWidth)
        {
            var ratio = (float)desiredWidth / (float)codec.Info.Width;
            var supportedScale = codec.GetScaledDimensions(ratio);

            SKEncodedImageFormat target = codec.EncodedFormat == SKEncodedImageFormat.Gif ? SKEncodedImageFormat.Gif : SKEncodedImageFormat.Png;

            var width = supportedScale.Width;
            var height = supportedScale.Height;
            var format = target.ToString();
            var name = $"{dbImage.Slug}_{width}_{height}";

            var blobRef = container.GetBlockBlobReference(name);
            var url = blobRef.StorageUri.PrimaryUri.ToString();

            using (var resultBitmap = SKBitmap.Decode(codec))
            using (var resizedBitmap = resultBitmap.Resize(new SKImageInfo(supportedScale.Width, supportedScale.Height), SKBitmapResizeMethod.Lanczos3))
            using (var resultImage = SKImage.FromBitmap(resizedBitmap))
            using (var imageStream = resultImage.Encode(target, 100).AsStream())
            {
                using (var outputSTream = await blobRef.OpenWriteAsync())
                {
                    await imageStream.CopyToAsync(outputSTream);
                }
            }

            var data = new AzureAdapterMetadata(this.account.BlobEndpoint.ToString(), container.Name, name, url);
            await this.imageData.SetImageLinks(new ImageLink(dbImage.Id, null, width, height, format, ImageAdapter.AzureFile, data.ToString()));
        }
    }
}
