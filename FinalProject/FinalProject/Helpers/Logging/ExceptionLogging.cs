using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NLog;
using System.Net;
using System.Text.Json;
using static FinalProject.Helpers.Logging.ExceptionLogging;
using ILogger = NLog.ILogger;

namespace FinalProject.Helpers.Logging
{
    public static class ExceptionLogging
    {
        public interface ILog
        {
            void Information(string message);
            void Warning(string message);
            void Debug(string message);
            void Error(string message);
        }

        public class LogNLog : ILog
        {
            private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
            public LogNLog()
            {
            }
            public void Debug(string message)
            {
                logger.Debug(message);
            }
            public void Error(string message)
            {
                logger.Error(message);
            }
            public void Information(string message)
            {
                logger.Info(message);
            }
            public void Warning(string message)
            {
                logger.Warn(message);
            }
        }

        public class ErrorDetails
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public override string ToString()
            {
                return JsonSerializer.Serialize(this);
            }
        }
    }

    public static class ExceptionExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILog logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.Error($"Something went wrong {contextFeature.Error}");
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal server error"
                        }.ToString());
                    }
                });
            });
        }
    }
}
