using System;

namespace CatiLyfe.Common.Utilities
{
    /// <summary>
    /// A generic disposable class.
    /// </summary>
    public class GenericDisposable : IDisposable
    {
        /// <summary>
        /// Action to run on dispose.
        /// </summary>
        private readonly Action onDispose;

        /// <summary>
        /// Initializs a new instance of the <see cref="GenericDisposable"/> class.
        /// </summary>
        /// <param name="onDispose"></param>
        private GenericDisposable(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        /// <summary>
        /// Create a generic disposable.
        /// </summary>
        /// <param name="onDispose">The action to execute on dispose.</param>
        /// <returns>The disposable.</returns>
        public static IDisposable Create(Action onDispose)
        {
            return new GenericDisposable(onDispose);
        }

        /// <summary>
        /// Execute the dispose.
        /// </summary>
        public void Dispose()
        {
            this.onDispose();
        }
    }
}
