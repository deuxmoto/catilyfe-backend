using CatiLyfe.DataLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatiLyfe.Backend.ImageServices
{
    public static class ImageUploaderFactory
    {
        public static IImageUploader Create(ICatiImageDataLayer imageData, string storageAccountConnection)
        {
            return new AzureStorageImageUploader(imageData, storageAccountConnection);
        }
    }
}
