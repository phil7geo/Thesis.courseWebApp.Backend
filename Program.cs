using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thesis.courseWebApp.Backend.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Thesis.courseWebApp.Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                 IConfiguration configuration = context.Configuration;
                 //services.AddTransient<EmailService>();

                 services.AddControllers()
                     .AddJsonOptions(options =>
                     {
                         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                     });
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

                 services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = configuration["Jwt:Issuer"],
                          ValidAudience = configuration["Jwt:Audience"],
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
                      };
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
                    app.UseAuthentication();
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


