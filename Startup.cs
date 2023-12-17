using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Thesis.courseWebApp.Backend
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();  // Add MVC services or other necessary services
			services.AddCors(options =>
			{
				options.AddPolicy("AllowReactFrontend",
					builder => builder.WithOrigins("http://http://localhost:3000")
									  .AllowAnyHeader()
									  .AllowAnyMethod());
			});
			// Add other services as needed
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			//app.UseCors(builder =>
			//{
			//	builder.AllowAnyOrigin()
			//		   .AllowAnyMethod()
			//		   .AllowAnyHeader();
			//});

			//app.UseCors("AllowReactFrontend");
			app.UseCors();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();  // Map controllers
			});
		}
	}
}
