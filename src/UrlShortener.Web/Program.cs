using Microsoft.EntityFrameworkCore;
using UrlShortener.Modules.Search;
using UrlShortener.Modules.Search.Controllers;
using UrlShortener.Modules.Urls;
using UrlShortener.Modules.Urls.Controllers;
using UrlShortener.Web.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(UrlsController).Assembly)
    .AddApplicationPart(typeof(SearchController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DbContext>(serviceProvider =>
    serviceProvider.GetRequiredService<AppDbContext>());

builder.Services.AddUrlsModule();
builder.Services.AddSearchModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
