
using Microsoft.EntityFrameworkCore;
using Repository;
using AutoMapper;
using DTOs;
using Microsoft.Extensions.DependencyInjection;
using Service;
using DotanBooks.Middlewares;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();


    builder.Services.AddScoped<IGetByCategoriesService, GetByCategoriesService>();
    builder.Services.AddScoped<IGetByCategoriesRepository, GetByCategoriesRepository>();
    builder.Services.AddScoped<ISearchBookService, SearchBookService>();
    builder.Services.AddScoped<ISearchBookRepository, SearchBookRepository>();


    builder.Services.AddDbContext<StoreContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.AddProfile<MappingProfiles>();
    });

    //חיבור ל client
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular",
            policy =>
            {
                policy.WithOrigins("http://localhost:53769")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    });



    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();//ההזרקה של במידלוור של ה 404 

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseCors("AllowAngular");

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
