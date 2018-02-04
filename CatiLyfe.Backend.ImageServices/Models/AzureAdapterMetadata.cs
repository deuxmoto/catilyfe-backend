using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace CatiLyfe.Backend.ImageServices.Models
{
    [DataContract]
    internal class AzureAdapterMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureAdapterMetadata"/> class.
        /// </summary>
        /// <param name="storageAccount">The storage account uri.</param>
        /// <param name="container">The container id.</param>
        /// <param name="name">The name of the file.</param>
        /// <param name="url">The url.</param>
        public AzureAdapterMetadata(string storageAccount, string container, string name, string url)
        {
            this.StorageAccount = storageAccount;
            this.Container = container;
            this.Name = name;
            this.Url = url;
        }

        /// <summary>
        /// The storage account.
        /// </summary>
        [DataMember]
        public string StorageAccount { get; private set; }

        /// <summary>
        /// The image contianer.
        /// </summary>
        [DataMember]
        public string Container { get; private set; }

        /// <summary>
        /// Name of the image.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the url.
        /// </summary>
        [DataMember]
        public string Url { get; private set; }

        /// <summary>
        /// Gets this object from a string.
        /// </summary>
        /// <param name="serialized">The serialized object.</param>
        /// <returns>The object.</returns>
        public static AzureAdapterMetadata Parse(string serialized)
        {
            var serializer = new DataContractSerializer(typeof(AzureAdapterMetadata));
            using (var memstream = new MemoryStream(Encoding.Unicode.GetBytes(serialized)))
            {
                return (AzureAdapterMetadata)serializer.ReadObject(memstream);
            }
        }

        /// <summary>
        /// Converts this object to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            var serializer = new DataContractSerializer(typeof(AzureAdapterMetadata));

            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, new XmlWriterSettings
            {
                Encoding = Encoding.Unicode
            }))
            {
                serializer.WriteObject(writer, this);
            }

            return builder.ToString();
        }
    }
}
