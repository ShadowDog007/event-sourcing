using Marten.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Transactions;

namespace ES.Core;

public static class WebApplicationExtensions
{
    public static void UseEventSourcingCore(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            opt.RoutePrefix = string.Empty;
        });

        // TODO - Setup auth
        // app.UseAuthentication();

        app.Use(async (_, next) =>
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
            await next();
            scope.Complete();
        });

        app.MapSwagger().AllowAnonymous();
    }
}
