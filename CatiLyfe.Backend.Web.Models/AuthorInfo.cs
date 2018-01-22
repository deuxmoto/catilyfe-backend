namespace CatiLyfe.Backend.Web.Models
{
    /// <summary>
    /// The author info.
    /// </summary>
    public class AuthorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorInfo"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public AuthorInfo(string name, int id)
        {
            this.Name = name;
            this.UserId = id;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        public int UserId { get; }
    }
}
