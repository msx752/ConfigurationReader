using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel.Bus;
using Shared.Kernel.Messages;
using System;

namespace ConfigurationLib.Dashboard.Middlewares
{
    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseExceptionHandler(errorApp =>
                 {
                     errorApp.Run(async context =>
                     {
                         try
                         {
                             var messageBus = context.RequestServices.GetRequiredService<IMessageBus>();
                             var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
                             var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                             if (exceptionFeature != null && exceptionFeature.Error != null)
                             {
                                 ExceptionLogEvent @event = new()
                                 {
                                     ApplicationName = configuration["AppName"],
                                     CorrelationId = Guid.NewGuid(),
                                     ExceptionMessage = exceptionFeature.Error.Message,
                                     ExceptionStackTrace = exceptionFeature.Error.StackTrace,
                                 };
                                 // exceptionFeature.Error.InnerException
                                 await messageBus.PublishEvent(@event);
                             }
                         }
                         catch (Exception e)
                         {
                         }
                     });
                 });

        }
    }
}
