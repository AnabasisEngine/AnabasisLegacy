using Anabasis.Core;
using Anabasis.Tasks;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Hosting.Hosting;

internal class AnabasisHostedService : IHostedService
{
    private readonly IServiceProvider       _provider;
    private readonly AnabasisWindow         _window;
    private readonly AnabasisGraphicsDevice _graphicsDevice;
    private readonly Thread                 _thread;

    public AnabasisHostedService(IServiceProvider provider, AnabasisWindow window, AnabasisGraphicsDevice graphicsDevice) {
        _provider = provider;
        _window = window;
        _graphicsDevice = graphicsDevice;
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
        AnabasisRunLoop loop = _provider.GetRequiredService<AnabasisRunLoop>();
        AnabasisTaskScheduler.MainThreadId = Environment.CurrentManagedThreadId;
        AnabasisGame[] game = new AnabasisGame[1];
        IServiceScope[] scope = new IServiceScope[1];
        AnabasisRunLoop.PlatformLoopHandlerDisposer[] taskRegistrations =
            new AnabasisRunLoop.PlatformLoopHandlerDisposer[8];
        PooledDelegates.Releaser[] taskClosures = new PooledDelegates.Releaser[8];
        try {
            for (int i = 0; i < 8; i++) {
                AnabasisPlatformLoopStep step = (AnabasisPlatformLoopStep)i;
                taskClosures[i] = PooledDelegates.GetPooledAction(AnabasisTaskScheduler.RunAction,
                    step, out Action action);
                taskRegistrations[i] = loop.RegisterHandler(step, -127, $"TaskScheduler.{step}", action);
            }

            using (PooledDelegates.GetPooledAction(g => g[0].Update(), game, out Action updateAction))
            using (PooledDelegates.GetPooledAction(g => g[0].Render(), game, out Action renderAction))
            using (PooledDelegates.GetPooledAction(arg => {
                       IServiceScope s = arg.scope[0] = _provider.CreateScope();
                       arg.game[0] = s.ServiceProvider.GetRequiredService<AnabasisGame>();
                       arg.game[0].DoLoad();
                   }, new { scope, game }, out Action loadAction))
            using (loop.RegisterHandler(AnabasisPlatformLoopStep.Initialization, 0, "IAnabasisGame.Load", loadAction))
            using (loop.RegisterHandler(AnabasisPlatformLoopStep.Update, 0, "IAnabasisGame.Update", updateAction))
            using (loop.RegisterHandler(AnabasisPlatformLoopStep.Render, 0, "IAnabasisGame.Render", renderAction)) {
                _window.Run(loop, _graphicsDevice, () => { scope[0].Dispose(); });
            }
        }
        finally {
            for (int i = 0; i < 8; i++) {
                taskRegistrations[i].Dispose();
                taskClosures[i].Dispose();
            }

            scope[0].Dispose();
        }
    }
}