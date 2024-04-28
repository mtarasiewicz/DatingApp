using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello world");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();