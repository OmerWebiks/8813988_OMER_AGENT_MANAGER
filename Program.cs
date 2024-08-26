using ManagementOfMossadAgentsAPI.api.Del;
using ManagementOfMossadAgentsAPI.api.MiddelWares.Global;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Add services to the container.
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ManagementOfMossadAgentsDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/Login"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<JwtValidationMiddleware>();
    }
);

app.UseAuthorization();

app.MapControllers();

app.Run();
