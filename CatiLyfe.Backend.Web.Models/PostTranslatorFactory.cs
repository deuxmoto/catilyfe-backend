using CatiLyfe.DataLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatiLyfe.Backend.Web.Models
{
    /// <summary>
    /// The static post translator factory.
    /// </summary>
    public static class PostTranslatorFactory
    {
        /// <summary>
        /// Creates a default post translator instance.
        /// </summary>
        /// <param name="authdata">The auth data retreiver.</param>
        /// <param name="transformer">The content transformer.</param>
        /// <returns>The post translator.</returns>
        public static IPostTranslator Create(ICatiAuthDataLayer authdata, IContentTransformer transformer)
        {
            return new PostTranslator(authdata, transformer);
        }
    }
}
