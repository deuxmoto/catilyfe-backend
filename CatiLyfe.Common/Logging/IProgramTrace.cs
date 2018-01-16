using System;
using System.Runtime.CompilerServices;

namespace CatiLyfe.Common.Logging
{
    public interface IProgramTrace
    {
        /// <summary>
        /// Trace a verbose message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="method">The method caller.</param>
        void TraceVerbose(string message, [CallerMemberName] string method = "");

        /// <summary>
        /// Trace a verbose message.
        /// </summary>
        /// <param name="message">The message if any.</param>
        /// <param name="method">The method caller.</param>
        IDisposable TraceMethod(string message = null, [CallerMemberName] string method = "");

        /// <summary>
        /// Trace a informational message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="method">The method caller.</param>
        void TraceInfo(string message, [CallerMemberName] string method = "");

        /// <summary>
        /// Trace a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception if any.</param>
        /// <param name="method">The method caller.</param>
        void TraceWarning(string message, Exception ex, [CallerMemberName] string method = "");

        /// <summary>
        /// Trace an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception if any.</param>
        /// <param name="method">The method caller.</param>
        void TraceError(string message, Exception ex = null, [CallerMemberName] string method = "");

        /// <summary>
        /// Trace a critical message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception if any.</param>
        /// <param name="method">The method caller.</param>
        void TraceCritical(string message, Exception ex = null, [CallerMemberName] string method = "");
    }
}
