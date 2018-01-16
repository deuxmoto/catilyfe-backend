using CatiLyfe.Common.Logging;
using CatiLyfe.Common.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CatiLyfe.Backend.Web.Core.Code.Trace
{
    public sealed class WebAppTrace : IProgramTrace
    {
        private readonly ILogger logger;

        public WebAppTrace(ILoggerFactory factory)
        {
            this.logger = factory.CreateLogger("CatiLyfe");
        }

        public void TraceCritical(string message, Exception ex = null, [CallerMemberName] string method = "")
        {
            this.logger.LogCritical(message);
        }

        public void TraceError(string message, Exception ex = null, [CallerMemberName] string method = "")
        {
            this.logger.LogError(message);
        }

        public void TraceInfo(string message, [CallerMemberName] string method = "")
        {
            this.logger.LogInformation(message);
        }

        public IDisposable TraceMethod(string message = null, [CallerMemberName] string method = "")
        {
            this.logger.LogDebug(method);
            return GenericDisposable.Create(() => { this.logger.LogDebug(method); });
        }

        public void TraceVerbose(string message, [CallerMemberName] string method = "")
        {
            this.logger.LogDebug(message);
        }

        public void TraceWarning(string message, Exception ex, [CallerMemberName] string method = "")
        {
            this.logger.LogWarning(message);
        }
    }
}
