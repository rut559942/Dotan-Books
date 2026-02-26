using Microsoft.EntityFrameworkCore;
using Repository;
using AutoMapper;
using DTOs;
using Microsoft.Extensions.DependencyInjection;
using Service;
using DotanBooks.Middlewares;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
    builder.Services.AddScoped<IBookByIdService, BookByIdService>();
    builder.Services.AddScoped<IBookByIdRepository, BookByIdRepository>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IPromotionService, PromotionService>();
    builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
    builder.Services.AddScoped<IAuthorService, AuthorService>();
    builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
    builder.Services.AddScoped<IManagementBookService, ManagementBookService>();
    builder.Services.AddScoped<IManagementBookRepository, ManagementBookRepository>();


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
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    });

    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ClockSkew = TimeSpan.Zero // מבטל את הדיליי המובנה של 5 דקות פקיעת תוקף
        };
    });

    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();//ההזרקה של במידלוור של ה 404 
                                             // Configure the HTTP request pipeline.

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();


    app.UseStaticFiles(new StaticFileOptions
    {
        ServeUnknownFileTypes = true
    });

    app.UseCors("AllowAngular");

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}