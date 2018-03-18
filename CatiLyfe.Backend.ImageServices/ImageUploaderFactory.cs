using CatiLyfe.DataLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatiLyfe.Backend.ImageServices
{
    public static class ImageUploaderFactory
    {
        /// <summary>
        /// Creates the image uploaded.
        /// </summary>
        /// <param name="imageData">The iamge data layer</param>
        /// <param name="storageAccountConnection">The storage account conntection</param>
        /// <param name="imageWidths">The image widths.</param>
        /// <returns></returns>
        public static IImageUploader Create(ICatiImageDataLayer imageData, string storageAccountConnection, int[] imageWidths)
        {
            return new AzureStorageImageUploader(imageData, storageAccountConnection, imageWidths);
        }
    }
}
