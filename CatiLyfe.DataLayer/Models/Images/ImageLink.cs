using System;
using System.Collections.Generic;
using System.Text;

namespace CatiLyfe.DataLayer.Models.Images
{
    public class ImageLink
    {
        public ImageLink(int? imageid, int? linkId, int width, int height, string format, ImageAdapter adapter, string metadata)
        {
            this.ImageId = imageid;
            this.LinkId = linkId;
            this.Width = width;
            this.Height = height;
            this.Format = format;
            this.Adapter = adapter;
            this.Metadata = metadata;
        }

        public int? ImageId { get; }

        public int? LinkId { get; }

        public int Width { get; }

        public int Height { get; }

        public string Format { get; }

        public ImageAdapter Adapter { get; }

        public string Metadata { get; }
    }
}
