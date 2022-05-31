using Anabasis.Core;
using Anabasis.Tasks;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anabasis.Hosting.Hosting;

public class AnabasisHostedService : IHostedService
{
    private readonly IServiceProvider  _provider;
    private readonly Thread            _thread;

    public AnabasisHostedService(IServiceProvider provider) {
        _provider = provider;
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
        AnabasisTaskHelper.MainThreadId = Environment.CurrentManagedThreadId;
        AnabasisGame? game = null;
        IServiceScope? scope = null;
        try {
            AnabasisRunLoop.PlatformLoopHandlerDisposer[] taskRegistrations = new AnabasisRunLoop.PlatformLoopHandlerDisposer[8];
            PooledDelegates.Releaser[] taskClosures = new PooledDelegates.Releaser[8];
            try { 
                for (int i = 0; i < 8; i++) {
                    AnabasisPlatformLoopStep step = (AnabasisPlatformLoopStep)i;
                    taskClosures[i] = PooledDelegates.GetPooledAction(AnabasisTaskHelper.RunAction,
                        step, out Action action);
                    taskRegistrations[i] = loop.RegisterHandler(step, -127, $"TaskScheduler.{step}", action);
                }
                using (loop.RegisterHandler(AnabasisPlatformLoopStep.Initialization, 0, "IAnabasisGame.Load", () => {
                           scope = _provider.CreateScope();
                           game = scope.ServiceProvider.GetRequiredService<AnabasisGame>();
                           game.DoLoad();
                       }))
                using (loop.RegisterHandler(AnabasisPlatformLoopStep.Update, 0, "IAnabasisGame.Update",
                           () => game?.Update()))
                using (loop.RegisterHandler(AnabasisPlatformLoopStep.Render, 0, "IAnabasisGame.Render",
                           () => game?.Render())) {
                    // _platform.Window.Run(loop, _provider.GetRequiredService<IAnabasisTime>(), () => { scope?.Dispose(); });
                    throw new NotImplementedException();
                }
            }
            finally {
                for (int i = 0; i < 8; i++) {
                    taskRegistrations[i].Dispose();
                    taskClosures[i].Dispose();
                }
            }
        }
        finally {
            scope?.Dispose();
        }
    }
}