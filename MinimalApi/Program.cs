
using Microsoft.EntityFrameworkCore;
using MinimalApi.Data;
using MinimalApi.Models;

namespace MinimalApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefCon"));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseAuthorization();

            app.MapGet("/", (ApplicationDbContext context) =>
            {
                var products = context.Products.ToList();
                return Results.Ok(products);

            });
            app.MapPost("/", (ApplicationDbContext context, Models.Product product) =>
            {
                context.Products.Add(product);
                context.SaveChanges();
                return Results.Ok(product);

            });
            app.MapPut("/", (ApplicationDbContext context, Product product) =>
            {
                var Product = context.Products.Find(product.Id);
                Product.Name = product.Name;
                Product.Description = product.Description;
                Product.Price = product.Price;
                Product.Quantity = product.Quantity;
                context.SaveChanges();
                return Results.Ok(product);

            });
            app.MapDelete("/", (ApplicationDbContext context, int id) =>
            {
                var product = context.Products.Find(id);
                context.Products.Remove(product);
                context.SaveChanges();
                return Results.Ok(product);

            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            app.Run();
        }
    }
}
