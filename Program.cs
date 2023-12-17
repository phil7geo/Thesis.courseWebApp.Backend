var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//class Program
//{
//    static void Main(string[] args)
//    {
//        var serviceProvider = new ServiceCollection()
//            .AddDbContext<appDbContext>(options =>
//                options.UseNpgsql("DbConnection"))
//            .BuildServiceProvider();

//        // Use the serviceProvider to access your DbContext
//    }
//}
