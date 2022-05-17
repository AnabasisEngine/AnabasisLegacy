// See https://aka.ms/new-console-template for more information

using Anabasis;
using Anabasis.Abstractions;
using Anabasis.Builder;
using Anabasis.Platform.Silk;
using BasicSample;
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
        .ConfigureServices(s => s.AddSingleton<IAnabasisGame, Game>())
        .Build()
        .RunAsync();
    return 0;
}
catch (Exception e) {
    Log.Fatal(e, "Error running host");
    return e.HResult;
}