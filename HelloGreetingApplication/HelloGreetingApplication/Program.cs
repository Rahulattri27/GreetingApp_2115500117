using NLog;
using NLog.Web;
using System;
using Microsoft.Extensions.Hosting;
using BusinessLayer.Interface;
using BusinessLayer.Services;
using RepositoryLayer.Interface;
using RepositoryLayer.Services;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using Middlewares.GlobalExceptionHandler;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Hashing;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

//Setup the Nlog from nlog.config and start the Nlog
var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    logger.Info("Starting the application");
    //make the builder
    var builder = WebApplication.CreateBuilder(args);
    //Add Database context
    builder.Services.AddDbContext<HelloGreetingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sqlserver")));


    //this ensures that default logging providers are not used
    builder.Logging.ClearProviders();

    //it ensures that Nlog is used for logs
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddScoped<IGreetingBL, GreetingBL>();
    builder.Services.AddScoped<IGreetingRL, GreetingRL>();
    builder.Services.AddScoped<IUserBL, UserBL>();
    builder.Services.AddScoped<IUserRL, UserRL>();
    builder.Services.AddScoped<Password_Hash>();
    builder.Services.AddControllers();

    // Add Swagger services
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Greetings API",
            Version = "v1",
            Description = "An API to manage Greetings"
        });
        // Add JWT Authentication to Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your token in the text input below."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });


        // Enable XML comments if the XML file exists
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });
    var jwt = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwt["SecretKey"]);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

    builder.Services.AddAuthorization();


    var app = builder.Build();

    app.UseMiddleware<GlobalExceptionMiddleware>();

    // Configure the HTTP request pipeline.

    app.UseHttpsRedirection();
    app.UseAuthentication();
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

