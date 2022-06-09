using Anabasis.Ascension;
using Anabasis.Core;
using Anabasis.Hosting;
using Anabasis.Hosting.Builder;
using Anabasis.ImageSharp;
using AscensionSample;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try {
    await AnabasisApp.CreateBuilder(args)
        .ConfigureHost(h => h.UseSerilog((ctx, svc, conf) => {
            conf
                .MinimumLevel.Verbose()
                .ReadFrom.Services(svc)
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        }))
        .ConfigureServices(s => {
            s.AddAscension();
            s.AddScoped<ImageSharpLoader>();
            s.AddScoped<AnabasisGame, Game>();
        })
        .Build()
        .RunAsync();
    return 0;
}
catch (Exception e) {
    Log.Fatal(e, "Error running host");
    return e.HResult;
}