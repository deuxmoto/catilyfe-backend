using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CatiLyfe.Common.Utilities
{
    public static class ListUtilities
    {
        /// <summary>
        /// Gets the collection as a readonly.
        /// </summary>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <param name="self">The list to convert.</param>
        /// <returns>The readonly collection</returns>
        public static IReadOnlyCollection<T> AsReadonly<T>(this IEnumerable<T> self)
        {
            if(self is IList<T> lst)
            {
                return new ReadOnlyCollection<T>(lst);
            }

            return new ReadOnlyCollection<T>(self.ToList());
        }
    }
}
