using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Net.Security;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

class Program
{
    const string serviceName = "monitored-docker-web-service";
    const string serviceVersion = "1.0.0";

    private readonly Tracer _tracer;

    public Program()
    {
        _tracer = TracerProvider.Default.GetTracer(serviceName);
    }

    private async Task DefaultDelegate(HttpContext context)
    {
        await context.Response.WriteAsync("Homepage + Did the change go through?");
    }
    private async Task HelloWorldDelegate(HttpContext context)
    {
        await context.Response.WriteAsync("Hello World!");
    }

    private async Task GoodbyeWorldDelegate(HttpContext context)
    {
        await context.Response.WriteAsync("Goodbye World!");
    }

    static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenTelemetry().WithTracing
        (
            tcb=>
            {
                tcb
                .AddSource(serviceName)
                .SetResourceBuilder
                (
                    ResourceBuilder.CreateDefault()
                    .AddService
                    (
                        serviceName: serviceName
                        , serviceVersion: serviceVersion
                    )
                )
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter();
            }
        );

        Program instance = new Program();

        WebApplication app = builder.Build();

        app.MapGet("/", instance.DefaultDelegate);
        app.MapGet("/hello", instance.HelloWorldDelegate);
        app.MapGet("/goodbye", instance.GoodbyeWorldDelegate);

        app.Run();
    }
}
