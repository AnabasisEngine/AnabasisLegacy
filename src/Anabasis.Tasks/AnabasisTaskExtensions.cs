using System.Diagnostics.CodeAnalysis;

namespace Anabasis.Tasks;

public static class AnabasisTaskExtensions
{
    public static Task<T> AsTask<T>(this AnabasisTask<T> task) {
        try {
            AnabasisTask<T>.Awaiter awaiter;
            try {
                awaiter = task.GetAwaiter();
            }
            catch (Exception ex) {
                return Task.FromException<T>(ex);
            }

            if (awaiter.IsCompleted) {
                try {
                    T result = awaiter.GetResult();
                    return Task.FromResult(result);
                }
                catch (Exception ex) {
                    return Task.FromException<T>(ex);
                }
            }

            TaskCompletionSource<T> tcs = new();

            awaiter.SourceOnCompleted(state => {
                using StateTuple<TaskCompletionSource<T>, AnabasisTask<T>.Awaiter> tuple =
                    (StateTuple<TaskCompletionSource<T>, AnabasisTask<T>.Awaiter>)state!;
                (TaskCompletionSource<T> inTcs, AnabasisTask<T>.Awaiter inAwaiter) = tuple;
                try {
                    T result = inAwaiter.GetResult();
                    inTcs.SetResult(result);
                }
                catch (Exception ex) {
                    inTcs.SetException(ex);
                }
            }, StateTuple.Create(tcs, awaiter));

            return tcs.Task;
        }
        catch (Exception ex) {
            return Task.FromException<T>(ex);
        }
    }

    public static Task AsTask(this AnabasisTask task) {
        try {
            AnabasisTask.Awaiter awaiter;
            try {
                awaiter = task.GetAwaiter();
            }
            catch (Exception ex) {
                return Task.FromException(ex);
            }

            if (awaiter.IsCompleted) {
                try {
                    awaiter.GetResult(); // check token valid on Succeeded
                    return Task.CompletedTask;
                }
                catch (Exception ex) {
                    return Task.FromException(ex);
                }
            }

            TaskCompletionSource<object> tcs = new();

            awaiter.SourceOnCompleted(state => {
                using StateTuple<TaskCompletionSource<object>, AnabasisTask.Awaiter> tuple =
                    (StateTuple<TaskCompletionSource<object>, AnabasisTask.Awaiter>)state!;
                (TaskCompletionSource<object> inTcs, AnabasisTask.Awaiter inAwaiter) = tuple;
                try {
                    inAwaiter.GetResult();
                    inTcs.SetResult(null!);
                }
                catch (Exception ex) {
                    inTcs.SetException(ex);
                }
            }, StateTuple.Create(tcs, awaiter));

            return tcs.Task;
        }
        catch (Exception ex) {
            return Task.FromException(ex);
        }
    }

    public static void Forget(this AnabasisTask task) {
        AnabasisTask.Awaiter awaiter = task.GetAwaiter();
        if (awaiter.IsCompleted) {
            try {
                awaiter.GetResult();
            }
            catch (Exception ex) {
                AnabasisTaskScheduler.PublishUnobservedTaskException(ex);
            }
        } else {
            awaiter.SourceOnCompleted(state => {
                using StateTuple<AnabasisTask.Awaiter> t = (StateTuple<AnabasisTask.Awaiter>)state!;
                try {
                    t.Item1.GetResult();
                }
                catch (Exception ex) {
                    AnabasisTaskScheduler.PublishUnobservedTaskException(ex);
                }
            }, StateTuple.Create(awaiter));
        }
    }

    [SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters")]
    public static void Forget(this AnabasisTask task, Action<Exception> exceptionHandler,
        bool handleExceptionOnMainThread = true) {
        if (exceptionHandler == null!) {
            Forget(task);
        } else {
            ForgetCoreWithCatch(task, exceptionHandler, handleExceptionOnMainThread).Forget();
        }
    }

    static async AnabasisVoidTask ForgetCoreWithCatch(AnabasisTask task, Action<Exception> exceptionHandler,
        bool handleExceptionOnMainThread) {
        try {
            await task;
        }
        catch (Exception ex) {
            try {
                if (handleExceptionOnMainThread) {
                    await AnabasisTask.SwitchToMainThread();
                }

                exceptionHandler(ex);
            }
            catch (Exception ex2) {
                AnabasisTaskScheduler.PublishUnobservedTaskException(ex2);
            }
        }
    }

    public static void Forget<T>(this AnabasisTask<T> task) {
        AnabasisTask<T>.Awaiter awaiter = task.GetAwaiter();
        if (awaiter.IsCompleted) {
            try {
                awaiter.GetResult();
            }
            catch (Exception ex) {
                AnabasisTaskScheduler.PublishUnobservedTaskException(ex);
            }
        } else {
            awaiter.SourceOnCompleted(state => {
                using StateTuple<AnabasisTask<T>.Awaiter> t = (StateTuple<AnabasisTask<T>.Awaiter>)state!;
                try {
                    t.Item1.GetResult();
                }
                catch (Exception ex) {
                    AnabasisTaskScheduler.PublishUnobservedTaskException(ex);
                }
            }, StateTuple.Create(awaiter));
        }
    }

    [SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters")]
    public static void Forget<T>(this AnabasisTask<T> task, Action<Exception> exceptionHandler,
        bool handleExceptionOnMainThread = true) {
        if (exceptionHandler == null!) {
            task.Forget();
        } else {
            ForgetCoreWithCatch(task, exceptionHandler, handleExceptionOnMainThread).Forget();
        }
    }

    static async AnabasisVoidTask ForgetCoreWithCatch<T>(AnabasisTask<T> task, Action<Exception> exceptionHandler,
        bool handleExceptionOnMainThread) {
        try {
            await task;
        }
        catch (Exception ex) {
            try {
                if (handleExceptionOnMainThread) {
                    await AnabasisTask.SwitchToMainThread();
                }

                exceptionHandler(ex);
            }
            catch (Exception ex2) {
                AnabasisTaskScheduler.PublishUnobservedTaskException(ex2);
            }
        }
    }

    public static async AnabasisTask ContinueWith<T>(this AnabasisTask<T> task, Action<T> continuationFunction) =>
        continuationFunction(await task);

    public static async AnabasisTask ContinueWith<T>(this AnabasisTask<T> task,
        Func<T, AnabasisTask> continuationFunction) =>
        await continuationFunction(await task);

    public static async AnabasisTask<TR> ContinueWith<T, TR>(this AnabasisTask<T> task,
        Func<T, TR> continuationFunction) =>
        continuationFunction(await task);

    public static async AnabasisTask<TR> ContinueWith<T, TR>(this AnabasisTask<T> task,
        Func<T, AnabasisTask<TR>> continuationFunction) =>
        await continuationFunction(await task);

    public static async AnabasisTask ContinueWith(this AnabasisTask task, Action continuationFunction) {
        await task;
        continuationFunction();
    }

    public static async AnabasisTask ContinueWith(this AnabasisTask task, Func<AnabasisTask> continuationFunction) {
        await task;
        await continuationFunction();
    }

    public static async AnabasisTask<T> ContinueWith<T>(this AnabasisTask task, Func<T> continuationFunction) {
        await task;
        return continuationFunction();
    }

    public static async AnabasisTask<T> ContinueWith<T>(this AnabasisTask task,
        Func<AnabasisTask<T>> continuationFunction) {
        await task;
        return await continuationFunction();
    }

    public static async AnabasisTask<T> Unwrap<T>(this AnabasisTask<AnabasisTask<T>> task) => await await task;

    public static async AnabasisTask Unwrap(this AnabasisTask<AnabasisTask> task) => await await task;

    public static async AnabasisTask<T> Unwrap<T>(this Task<AnabasisTask<T>> task) => await await task;

    public static async AnabasisTask<T> Unwrap<T>(this Task<AnabasisTask<T>> task, bool continueOnCapturedContext) =>
        await await task.ConfigureAwait(continueOnCapturedContext);

    public static async AnabasisTask Unwrap(this Task<AnabasisTask> task) => await await task;

    public static async AnabasisTask Unwrap(this Task<AnabasisTask> task, bool continueOnCapturedContext) =>
        await await task.ConfigureAwait(continueOnCapturedContext);

    public static async AnabasisTask<T> Unwrap<T>(this AnabasisTask<Task<T>> task) => await await task;

    public static async AnabasisTask<T> Unwrap<T>(this AnabasisTask<Task<T>> task, bool continueOnCapturedContext) =>
        await (await task).ConfigureAwait(continueOnCapturedContext);

    public static async AnabasisTask Unwrap(this AnabasisTask<Task> task) => await await task;

    public static async AnabasisTask Unwrap(this AnabasisTask<Task> task, bool continueOnCapturedContext) =>
        await (await task).ConfigureAwait(continueOnCapturedContext);
}