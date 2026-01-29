using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Net.Security;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;

class Program
{

    private static async Task DefaultDelegate(HttpContext context)
    {
        await context.Response.WriteAsync("Homepage + Did the change go through?");
        Console.WriteLine("Default Visited");
    }
    private static async Task HelloWorldDelegate(HttpContext context)
    {
        Console.WriteLine("Hello Called");
        await context.Response.WriteAsync("Hello World!");
    }

    private static async Task GoodbyeWorldDelegate(HttpContext context)
    {
        Console.WriteLine("Goodbye Called");
        await context.Response.WriteAsync("Goodbye World!");
    }

    static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        WebApplication app = builder.Build();

        app.MapGet("/", DefaultDelegate);
        app.MapGet("/hello", HelloWorldDelegate);
        app.MapGet("/goodbye", GoodbyeWorldDelegate);

        app.Run();
    }
}
