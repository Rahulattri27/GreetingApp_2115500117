using NLog;
using NLog.Web;
using System;
using Microsoft.Extensions.Hosting;
using BusinessLayer.Interface;
using BusinessLayer.Services;

//Setup the Nlog from nlog.config and start the Nlog
var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    logger.Info("Starting the application");
    //make the builder
    var builder = WebApplication.CreateBuilder(args);

    //this ensures that default logging providers are not used
    builder.Logging.ClearProviders();

    //it ensures that Nlog is used for logs
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddScoped<IGreetingBL, GreetingBL>();

    builder.Services.AddControllers();

    // Add Swagger services
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseHttpsRedirection();

    app.UseAuthorization();

    // Enable Swagger UI only in development mode
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    app.MapControllers();

    app.Run();
}
catch(Exception ex)
{
    logger.Error(ex, "Application stopped due to an exception");
}
finally
{
    //Stop the Nlog 
    NLog.LogManager.Shutdown();
}

