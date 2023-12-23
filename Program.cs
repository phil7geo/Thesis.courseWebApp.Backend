//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//app.Run();
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thesis.courseWebApp.Backend.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;


public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
         .ConfigureWebHostDefaults(webBuilder =>
         {
             webBuilder.ConfigureServices((context, services) =>
             {
                 services.AddControllers();
                 services.AddDbContext<AppDbContext>(options =>
                     options.UseNpgsql(context.Configuration.GetConnectionString("DbConnection")));

                 services.AddCors(options =>
                    {
                        options.AddPolicy("AllowReactFrontend",
                            builder => builder
                                .WithOrigins("http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod());
                    });
                })
                .Configure(app =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

                    if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }

                    app.UseHttpsRedirection();
                    app.UseRouting();
                    app.UseCors("AllowReactFrontend");
                    app.UseAuthorization();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                        endpoints.MapGet("/", async context =>
                        {
                            await context.Response.WriteAsync("Hello from Backend!");
                        });
                    });
                });
            });
}


