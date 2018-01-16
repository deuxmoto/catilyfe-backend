namespace CatiLyfe.Backend.Web.Core.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CatiLyfe.Backend.Web.Models.Login;
    using CatiLyfe.Common.Security;
    using CatiLyfe.DataLayer;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using CatiLyfe.Backend.Web.Core.Code;
    using CatiLyfe.Common.Logging;
    using CatiLyfe.Common.Exceptions;

    /// <summary>
    /// The login controller.
    /// </summary>
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly ICatiAuthDataLayer authDataLayer;

        /// <summary>
        /// The log tracer.
        /// </summary>
        private readonly IProgramTrace tracer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController" class./>
        /// </summary>
        /// <param name="authDatalayer">The auth data layer.</param>
        /// <param name="trace">The tracer.</param>
        public LoginController(ICatiAuthDataLayer authDatalayer, IProgramTrace trace)
        {
            this.authDataLayer = authDatalayer;
            this.tracer = trace;
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Login([FromBody]LoginCredentials credentials)
        {

            bool result = false;
            try
            {
                result = await this.LoginUser(credentials);
            }
            catch (Exception ex)
            {
                result = false;
            }

            if (false == result)
            {
                this.tracer.TraceInfo($"Login failed for user Email: '{credentials.Email}'.");
                throw new AuthFailureException();
            }

            this.tracer.TraceInfo($"Login accepted for user Email: '{credentials.Email}'.");
            return this.NoContent();
        }

        [HttpDelete]
        [Authorize(Policy = "default")]
        public async Task<IActionResult> Logoff()
        {
            var user = this.GetUserAccessDetails();
            await this.authDataLayer.DeauthorizeToken(user.UserId, Convert.FromBase64String(user.Token));

            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            this.tracer.TraceInfo($"User has been logged off {user.Email}.");

            return this.NoContent();
        }

        /// <summary>
        /// Try to login a user.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>True on success.</returns>
        private async Task<bool> LoginUser(LoginCredentials credentials)
        {
            var user = (await this.authDataLayer.GetUser(
                ids: null,
                emails: new[] { credentials.Email },
                names: null,
                token: null)).FirstOrDefault();

            // Hash the password with the salt
            var hashedPassword = PasswordGenerator.HashPassword(user.Salt, credentials.Password);

            if (false == PasswordGenerator.IsMatch(user.Password, hashedPassword))
            {
                return false;
            }

            var token = PasswordGenerator.GenerateRandom(64);
            var tokenExpiration = DateTime.UtcNow + TimeSpan.FromDays(8);

            await this.authDataLayer.CreateToken(
                            user.Id.Value,
                            token,
                            tokenExpiration);

            var claims = new[]
                             {
                                 new Claim(ClaimTypes.AuthenticationMethod, "catilyfe"),
                                 new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToLongTimeString()),
                                 new Claim(ClaimTypes.Authentication, "yes"),
                                 new Claim(ClaimTypes.Expiration, tokenExpiration.ToLongTimeString()),
                                 new Claim(ClaimTypes.Hash, Convert.ToBase64String(token)),
                             };
            var identity = new ClaimsIdentity(claims, "catilyfe");

            var principal = new ClaimsPrincipal(identity);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });


            return true;
        }
    }
}
