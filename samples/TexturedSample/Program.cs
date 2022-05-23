using Anabasis;
using Anabasis.Abstractions;
using Anabasis.Builder;
using Anabasis.Platform.Silk;
using Anabasis.Platform.Silk.SixLabors.ImageSharp;
using Anabasis.Platform.SixLabors.ImageSharp;
using TexturedSample;
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
        .UsingPlatform<SilkPlatform>()
        .ConfigureServices(s => {
            s.AddScoped<IAnabasisGame, Game>();
            s.AddImageSharpUploader();
            s.AddSilkImageSharpSupport();
        })
        .Build()
        .RunAsync();
    return 0;
}
catch (Exception e) {
    Log.Fatal(e, "Error running host");
    return e.HResult;
}