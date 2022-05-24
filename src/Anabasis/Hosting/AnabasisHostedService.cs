using Anabasis.Abstractions;
using Anabasis.Platform.Abstractions;
using Anabasis.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Hosting;

public class AnabasisHostedService : IHostedService
{
    private readonly IServiceProvider  _provider;
    private readonly IAnabasisPlatform _platform;
    private readonly Thread            _thread;

    public AnabasisHostedService(IServiceProvider provider, IAnabasisPlatform platform) {
        _provider = provider;
        _platform = platform;
        _thread = new Thread(RunDelegate) {
            Name = "Anabasis Thread",
        };
        _thread.TrySetApartmentState(ApartmentState.STA);
    }


    public Task StartAsync(CancellationToken cancellationToken) {
        _thread.Start(this);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static readonly ParameterizedThreadStart RunDelegate = o => {
        if (o is AnabasisHostedService hostedService)
            hostedService.Run();
    };

    private void Run() {
        IAnabasisRunLoop loop = _provider.GetRequiredService<IAnabasisRunLoop>();
        AnabasisTaskManager taskManager = _provider.GetRequiredService<AnabasisTaskManager>();
        taskManager.MainThreadId = Environment.CurrentManagedThreadId;
        IAnabasisGame? game = null;
        IServiceScope? scope = null;
        try {
            using (loop.RegisterHandler(AnabasisPlatformLoopStep.Initialization, 0, "IAnabasisGame.Load", () => {
                       scope = _provider.CreateScope();
                       game = scope.ServiceProvider.GetRequiredService<IAnabasisGame>();
                       game.Load().Forget();
                   }))
            using (loop.RegisterHandler(AnabasisPlatformLoopStep.Update, 0, "IAnabasisGame.Update",
                       () => game?.Update()))
            using (loop.RegisterHandler(AnabasisPlatformLoopStep.Render, 0, "IAnabasisGame.Render",
                       () => game?.Render()))
                _platform.Window.Run(loop, _provider.GetRequiredService<IAnabasisTime>(), () => { scope?.Dispose(); });
        }
        finally {
            scope?.Dispose();
        }
    }
}