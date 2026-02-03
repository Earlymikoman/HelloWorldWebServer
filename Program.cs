using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Net.Security;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using CS397.Trace;
using CS397.Internal;
using CS397.Exporter;

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
        TelemetrySpan currentSpan = _tracer.StartSpan("DefaultDelegate");
        currentSpan.SetAttribute("http.method", context.Request.Method);
        currentSpan.SetAttribute("http.url", context.Request.Path);

        context.Response.Headers.Append("x-trace-id", currentSpan.Context.TraceId.ToString());

        try
        {
            await context.Response.WriteAsync("Homepage + Did the change go through?");   
        }
        catch(Exception e)
        {
            currentSpan.SetAttribute("error", true);
            currentSpan.SetAttribute("error.message", e.Message);
            currentSpan.SetAttribute("error.stacktrace", e.StackTrace);

            context.Response.StatusCode = 500;
        }
        finally
        {
            currentSpan.End();
        }
    }
    private async Task HelloWorldDelegate(HttpContext context)
    {
        TelemetrySpan currentSpan = _tracer.StartSpan("HelloWorldDelegate");
        currentSpan.SetAttribute("http.method", context.Request.Method);
        currentSpan.SetAttribute("http.url", context.Request.Path);

        context.Response.Headers.Append("x-trace-id", currentSpan.Context.TraceId.ToString());

        try
        {
            await context.Response.WriteAsync("Hello World!");   
        }
        catch(Exception e)
        {
            currentSpan.SetAttribute("error", true);
            currentSpan.SetAttribute("error.message", e.Message);
            currentSpan.SetAttribute("error.stacktrace", e.StackTrace);

            context.Response.StatusCode = 500;
        }
        finally
        {
            currentSpan.End();
        }
    }

    private async Task GoodbyeWorldDelegate(HttpContext context)
    {
        TelemetrySpan currentSpan = _tracer.StartSpan("GoodbyeWorldDelegate");
        currentSpan.SetAttribute("http.method", context.Request.Method);
        currentSpan.SetAttribute("http.url", context.Request.Path);

        context.Response.Headers.Append("x-trace-id", currentSpan.Context.TraceId.ToString());

        try
        {
            int x = 0;
            int y = 1/x;

            await context.Response.WriteAsync("Goodbye World!");
        }
        catch(Exception e)
        {
            currentSpan.SetAttribute("error", true);
            currentSpan.SetAttribute("error.message", e.Message);
            currentSpan.SetAttribute("error.stacktrace", e.StackTrace);

            context.Response.StatusCode = 500;
        }
        finally
        {
            currentSpan.End();
        }
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
                .AddJsonConsoleExporter();
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
