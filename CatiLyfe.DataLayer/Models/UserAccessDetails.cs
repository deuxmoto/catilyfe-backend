namespace CatiLyfe.DataLayer.Models
{
    /// <summary>
    /// The user access details.
    /// </summary>
    public class UserAccessDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccessDetails"/> class.
        /// </summary>
        /// <param name="userid"></param>
        public UserAccessDetails(int userid, string token, string email)
        {
            this.UserId = userid;
            this.Token = token;
            this.Email = email;
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Gets the user token.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the user email.
        /// </summary>
        public string Email { get; }
    }
}
