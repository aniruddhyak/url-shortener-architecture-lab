using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using UrlShortener.Modules.Search;
using UrlShortener.Modules.Search.Controllers;
using UrlShortener.Modules.Urls;
using UrlShortener.Modules.Urls.Controllers;
using UrlShortener.Web.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApplicationPartManager(manager =>
    {
        manager.ApplicationParts.Clear();
        manager.ApplicationParts.Add(new AssemblyPart(typeof(UrlsController).Assembly));
        manager.ApplicationParts.Add(new AssemblyPart(typeof(SearchController).Assembly));
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DbContext>(serviceProvider =>
    serviceProvider.GetRequiredService<AppDbContext>());
builder.Services.AddUrlsModule();
builder.Services.AddSearchModule();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
