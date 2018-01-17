using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CatiLyfe.DataLayer.Models.Images
{
    public class Image
    {

        public Image(int? id, string slug, string description, IEnumerable<ImageLink> links)
        {
            this.Id = id;
            this.Slug = slug;
            this.Description = description;
            this.Links = new List<ImageLink>(links);
        }

        public int? Id { get;}

        public string Slug { get; }

        public string Description { get; }

        public IList<ImageLink> Links { get; }
    }
}
